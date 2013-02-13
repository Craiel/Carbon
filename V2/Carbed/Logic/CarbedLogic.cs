using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

using Carbed.Contracts;
using Carbed.Views;

using Carbon.Editor.Contracts;
using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;
using Carbon.Project.Resource;

using Core.Utils.Contracts;

namespace Carbed.Logic
{
    public class CarbedLogic : CarbedBase, ICarbedLogic
    {
        private readonly XmlSerializer projectSerializer;
        private readonly ICarbonBuilder carbonBuilder;
        private readonly IEngineFactory engineFactory;
        private readonly ILog log;
        
        private SourceProject project;
        private IContentManager projectContent;
        private IResourceManager projectResources;

        private string tempLocation;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbedLogic(IEngineFactory factory)
        {
            this.engineFactory = factory;
            this.log = factory.Get<ICarbedLog>().AquireContextLog("Logic");
            this.carbonBuilder = factory.Get<ICarbonBuilder>();
            this.carbonBuilder.ProgressChanged += this.OnBuilderProgressChanged;

            this.projectSerializer = new XmlSerializer(typeof(SourceProject));
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event ProjectChangedEventHandler ProjectChanged;

        public SourceProject Project
        {
            get
            {
                return this.project;
            }
        }

        public IContentManager ProjectContent
        {
            get
            {
                return this.projectContent;
            }
        }

        public void NewProject()
        {
            this.project = new SourceProject();
            this.tempLocation = Path.GetTempPath();
            this.InitializeProject(this.tempLocation);
            this.NotifyProjectChanged();
        }

        public void CloseProject()
        {
            if (this.project != null)
            {
                this.project = null;

                // Todo: this will probably cause issues if resources are still in use somewhere
                // First clear up the content since that relies on the resource manager
                this.projectContent.Dispose();
                this.projectContent = null;

                this.projectResources.Dispose();
                this.projectResources = null;
            }

            this.tempLocation = null;
            this.NotifyProjectChanged();
        }

        public void OpenProject(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                throw new ArgumentException("Invalid File specified: " + file);
            }

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.project = (SourceProject)this.projectSerializer.Deserialize(stream);
                this.InitializeProject(Path.GetDirectoryName(file));
                this.NotifyProjectChanged();
            }
        }

        public void SaveProject(string file)
        {
            if (this.project == null)
            {
                throw new InvalidOperationException();
            }

            using (var stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                this.projectSerializer.Serialize(stream, this.project);
                
                if (!string.IsNullOrEmpty(this.tempLocation))
                {
                    // Todo: Copy the files from temp location to the project location
                    throw new NotImplementedException();
                }
            }
        }

        public void Build(string folder)
        {
            var task = new Task(() => this.carbonBuilder.Build(folder, this.project));
            TaskProgress.Message = string.Format("Building {0}", this.project.Name);
            new TaskProgress(new[] { task });
        }

        public object NewResource(EngineResourceType type)
        {
            switch (type)
            {
                case EngineResourceType.Font:
                    {
                        return new FontEntry();
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public object NewResource(ProjectResourceType type)
        {
            switch (type)
            {
                case ProjectResourceType.Model:
                    {
                        return new SourceModel();
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static void DoEvents(Dispatcher dispatcher)
        {
            var frame = new DispatcherFrame(true);
            dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (SendOrPostCallback)delegate(object arg)
                {
                    var f = arg as DispatcherFrame;
                    if (f != null)
                    {
                        f.Continue = false;
                    }
                },
                frame);
            Dispatcher.PushFrame(frame);
        }

        public IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue)
        {
            if (primaryKeyValue == null)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void InitializeProject(string rootPath)
        {
            if (this.project == null)
            {
                throw new InvalidOperationException("null Project can not be initialized");
            }

            string resourcePath = Path.Combine(rootPath, "Data");
            this.projectResources = this.engineFactory.GetResourceManager(resourcePath);

            string contentPath = Path.Combine(rootPath, "Main.db");
            this.projectContent = this.engineFactory.GetContentManager(this.projectResources, new ResourceLink { Source = contentPath });
        }

        private void NotifyProjectChanged()
        {
            var handler = this.ProjectChanged;
            if (handler != null)
            {
                handler(this.project);
            }
        }

        private void OnBuilderProgressChanged(string message, int current, int max)
        {
            TaskProgress.CurrentMaxProgress = max;
            TaskProgress.CurrentProgress = current;
            TaskProgress.CurrentMessage = message;
        }
    }
}
