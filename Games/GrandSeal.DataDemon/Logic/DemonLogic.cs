namespace GrandSeal.DataDemon.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using Core.Engine.Contracts;
    using Core.Utils.Contracts;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Ninject;

    public class DemonLogic : IDemonLogic
    {
        private static readonly XmlSerializer ConfigSerializer = new XmlSerializer(typeof(DemonConfig));

        private readonly IEngineFactory factory;

        private readonly IList<IDemonOperation> operations;

        private readonly ILog log;

        private DemonConfig config;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonLogic(IEngineFactory factory)
        {
            this.factory = factory;
            this.log = factory.Get<IDemonLog>().AquireContextLog("Logic");

            this.operations = new List<IDemonOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TimeSpan RefreshInterval { get; private set; }

        public void Dispose()
        {
            foreach (IDemonOperation operation in this.operations)
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
                this.log.Error("Failed to de-serialize configuration", e);
                return false;
            }

            return this.CheckConfiguration();
        }

        public void Refresh()
        {
            lock (this.operations)
            {
                foreach (IDemonOperation operation in this.operations)
                {
                    operation.Refresh();
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckConfiguration()
        {
            if (this.config == null)
            {
                this.log.Error("No configuration loaded!");
                return false;
            }

            this.RefreshInterval = TimeSpan.FromMilliseconds(this.config.RefreshInterval);
            if (this.RefreshInterval < TimeSpan.FromSeconds(1))
            {
                this.log.Warning("Refresh is less then one second, this can cause severe stress on the system!");
            }
            
            // Treat as warning for now, demon might have other functions beside source conversion
            if (this.config.DataConversions == null || this.config.DataConversions.Length <= 0)
            {
                this.log.Warning("Configuration has no sources specified");
            }
            else
            {
                foreach (DemonConversionConfig conversion in this.config.DataConversions)
                {
                    if (!this.CheckDataConversion(conversion))
                    {
                        return false;
                    }

                    var operation = this.factory.GetDemonConversion(conversion);
                    this.operations.Add(operation);
                }
            }

            if (this.config.Builds == null || this.config.Builds.Length <= 0)
            {
                this.log.Warning("Configuration has no builds specified");
            }
            else
            {
                foreach (DemonBuildConfig build in this.config.Builds)
                {
                    if (!this.CheckBuild(build))
                    {
                        return false;
                    }

                    var operation = this.factory.GetDemonBuild(build);
                    this.operations.Add(operation);
                }
            }

            return true;
        }

        private bool CheckDataConversion(DemonConversionConfig conversion)
        {
            if (string.IsNullOrEmpty(conversion.Name))
            {
                this.log.Error("Conversion has no name");
                return false;
            }

            if (string.IsNullOrEmpty(conversion.SourcePath) || !Directory.Exists(conversion.SourcePath))
            {
                this.log.Error("Conversion source path is invalid: {0}", null, conversion.SourcePath ?? "Null");
                return false;
            }

            if (string.IsNullOrEmpty(conversion.TargetPath))
            {
                this.log.Error("Conversion target path is invalid");
                return false;
            }

            this.log.Info("Loaded conversion {0}", conversion.Name);
            return true;
        }

        private bool CheckBuild(DemonBuildConfig build)
        {
            if (string.IsNullOrEmpty(build.Name))
            {
                this.log.Error("Build has no name");
                return false;
            }

            if (string.IsNullOrEmpty(build.ProjectRoot) || !Directory.Exists(build.ProjectRoot))
            {
                this.log.Error("Build project root is invalid: {0}", null, build.ProjectRoot ?? "Null");
                return false;
            }

            this.log.Info("Loaded build {0}", build.Name);
            return true;
        }
    }
}
