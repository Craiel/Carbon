namespace GrandSeal.DataDemon.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    
    using GrandSeal.DataDemon.Contracts;

    public class DemonLogic : IDemonLogic
    {
        private static readonly XmlSerializer ConfigSerializer = new XmlSerializer(typeof(DemonConfig));

        private readonly IFactory factory;

        private readonly IList<IDemonOperation> parallelOperations;
        private readonly IList<IDemonOperation> sequentialOperations; 
        
        private readonly IDemonFileInfo fileInfo;

        private DemonConfig config;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonLogic(IFactory factory)
        {
            this.factory = factory;
            this.fileInfo = factory.Resolve<IDemonFileInfo>();

            this.parallelOperations = new List<IDemonOperation>();
            this.sequentialOperations = new List<IDemonOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TimeSpan RefreshInterval { get; private set; }

        public void Dispose()
        {
            foreach (IDemonOperation operation in this.parallelOperations)
            {
                operation.Dispose();
            }

            foreach (IDemonOperation operation in this.sequentialOperations)
            {
                operation.Dispose();
            }
        }

        public bool LoadConfig(string file)
        {
            try
            {
                using (var reader = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var newConfig = ConfigSerializer.Deserialize(reader) as DemonConfig;
                    this.config = newConfig;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Failed to de-serialize configuration", e);
                return false;
            }

            return this.CheckAndLoadConfiguration();
        }

        public void Refresh()
        {
            // Need to update file info first, this is done before all operations
            this.fileInfo.Refresh();

            // Now we execute everything that can be run in parallel
            var tasks = new Task[this.parallelOperations.Count];
            for (int i = 0; i < this.parallelOperations.Count; i++)
            {
                tasks[i] = new Task(this.parallelOperations[i].Refresh);
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            // Lastly we execute all requested builds and things that can not run before the previous things are done
            foreach (IDemonOperation operation in this.sequentialOperations)
            {
                operation.Refresh();
                operation.Process();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckAndLoadConfiguration()
        {
            if (this.config == null)
            {
                System.Diagnostics.Trace.TraceError("No configuration loaded!");
                return false;
            }

            this.RefreshInterval = TimeSpan.FromMilliseconds(this.config.RefreshInterval);
            if (this.RefreshInterval < TimeSpan.FromSeconds(1))
            {
                System.Diagnostics.Trace.TraceWarning("Refresh is less then one second, this can cause severe stress on the system!");
            }
            
            // Treat as warning for now, demon might have other functions beside source conversion
            if (this.config.SourceIncludes == null || this.config.SourceIncludes.Length <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("Configuration has no sources specified");
            }
            else
            {
                foreach (DemonInclude include in this.config.SourceIncludes)
                {
                    if (!this.CheckInclude(include))
                    {
                        return false;
                    }

                    this.fileInfo.AddSourceInclude(include.Path);
                }

                foreach (DemonInclude include in this.config.IntermediateIncludes)
                {
                    if (!this.CheckInclude(include))
                    {
                        return false;
                    }

                    this.fileInfo.AddIntermediateInclude(include.Path);
                }
            }

            if (this.config.Builds == null || this.config.Builds.Length <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("Configuration has no builds specified");
            }
            else
            {
                foreach (DemonBuildConfig build in this.config.Builds)
                {
                    if (!this.CheckBuild(build))
                    {
                        return false;
                    }

                    var operation = this.factory.Resolve<IDemonBuild>();
                    operation.SetConfig(build);
                    this.sequentialOperations.Add(operation);
                }
            }

            return true;
        }

        private bool CheckInclude(DemonInclude include)
        {
            if (string.IsNullOrEmpty(include.Path))
            {
                System.Diagnostics.Trace.TraceError("Conversion source path is invalid: {0}", null, include.Path ?? "Null");
                return false;
            }

            System.Diagnostics.Trace.TraceInformation("Loaded include {0}", include.Path);
            return true;
        }

        private bool CheckBuild(DemonBuildConfig build)
        {
            if (string.IsNullOrEmpty(build.Name))
            {
                System.Diagnostics.Trace.TraceError("Build has no name");
                return false;
            }

            if (string.IsNullOrEmpty(build.ProjectRoot) || !Directory.Exists(build.ProjectRoot))
            {
                System.Diagnostics.Trace.TraceError("Build project root is invalid: {0}", null, build.ProjectRoot ?? "Null");
                return false;
            }

            System.Diagnostics.Trace.TraceInformation("Loaded build {0}", build.Name);
            return true;
        }
    }
}
