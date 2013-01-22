using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

using Carbed.Contracts;
using Carbed.Views;

using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;
using Carbon.Engine.Resource;
using Carbon.Project.Resource;

namespace Carbed.Logic
{
    using Carbon.Editor.Contracts;
    
    public class CarbedLogic : CarbedBase, ICarbedLogic
    {
        private readonly IEngineFactory factory;
        private readonly XmlSerializer projectSerializer;
        private readonly ICarbonBuilder carbonBuilder;
        
        private SourceProject project;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbedLogic(IEngineFactory factory)
        {
            this.factory = factory;
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

        public void NewProject()
        {
            this.project = new SourceProject();
            this.NotifyProjectChanged();
        }

        public void CloseProject()
        {
            this.project = null;
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
            }
        }

        public void Build(string folder)
        {
            var task = new Task(() => this.carbonBuilder.Build(folder, this.project));
            TaskProgress.Message = string.Format("Building {0}", this.project.Name);
            new TaskProgress(new[] { task });
        }

        public object NewResource(EngineResourceType type, string name)
        {
            switch (type)
            {
                case EngineResourceType.TextureFont:
                    {
                        return new SourceTextureFont { Name = name };
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public object NewResource(ProjectResourceType type, string name)
        {
            switch (type)
            {
                case ProjectResourceType.Model:
                    {
                        return new SourceModel { Name = name };
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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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
