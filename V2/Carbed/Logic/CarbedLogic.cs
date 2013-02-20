using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;

using Carbed.Contracts;
using Carbed.Views;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils.Contracts;

namespace Carbed.Logic
{
    public class CarbedLogic : CarbedBase, ICarbedLogic
    {
        private readonly IEngineFactory engineFactory;
        private readonly ILog log;
        private readonly IViewModelFactory viewModelFactory;

        private readonly List<IMaterialViewModel> materials;
        
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
            this.viewModelFactory = factory.Get<IViewModelFactory>();

            this.materials = new List<IMaterialViewModel>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event ProjectChangedEventHandler ProjectChanged;
        
        public bool HasProjectLoaded
        {
            get
            {
                return this.projectContent != null;
            }
        }

        public IReadOnlyCollection<IMaterialViewModel> Materials
        {
            get
            {
                return this.materials.AsReadOnly();
            }
        }

        public void NewProject()
        {
            this.CloseProject();
            this.tempLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            this.InitializeProject(this.tempLocation);
            this.NotifyProjectChanged();
        }

        public void CloseProject()
        {
            this.Unload();

            if (this.projectContent != null)
            {
                // Todo: this will probably cause issues if resources are still in use somewhere
                // First clear up the content since that relies on the resource manager
                this.projectContent.Dispose();
                this.projectContent = null;
            }

            if (this.projectResources != null)
            {

                this.projectResources.Dispose();
                this.projectResources = null;
            }

            this.tempLocation = null;
            this.NotifyProjectChanged();
        }

        public void OpenProject(string path)
        {
            this.CloseProject();

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid Path specified: " + path);
            }

            this.InitializeProject(Path.GetDirectoryName(path));
            this.Reload();
            this.NotifyProjectChanged();
        }

        public void SaveProject(string file)
        {
            this.log.Warning("Saving into different file is not fully supported yet!");
            /*
             * Todo:
             * - Choose a target path instead of file
             * - Move the Resources from the temporary folder as well
             * - Cleanup the temporary data
             */
            ResourceLink currentRoot = this.projectContent.Root;
            ResourceLink newRoot = new ResourceLink { Path = file };

            this.CloseProject();

            // Move the database file over to our new location
            if (File.Exists(currentRoot.Path))
            {
                File.Copy(currentRoot.Path, newRoot.Path);
            }
            
            this.OpenProject(file);
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

        public void Reload()
        {
            this.Unload();

            ContentQueryResult<MaterialEntry> materialData = this.projectContent.TypedLoad(new ContentQuery<MaterialEntry>());
            var materialEntries = materialData.ToList<MaterialEntry>();
            foreach (MaterialEntry entry in materialEntries)
            {
                IMaterialViewModel vm = this.viewModelFactory.GetMaterialViewModel(entry);
                // Todo: Move this into a seperate task into the loading vm
                vm.Load();
                this.materials.Add(vm);
            }
        }

        public IMaterialViewModel AddMaterial()
        {
            var vm = this.viewModelFactory.GetMaterialViewModel(new MaterialEntry());
            this.materials.Insert(0, vm);
            return vm;
        }

        public void Save(IMaterialViewModel material)
        {
            if (this.projectContent == null)
            {
                throw new InvalidOperationException();
            }

            material.Save(this.projectContent);
        }

        public void Delete(IMaterialViewModel material)
        {
            throw new NotImplementedException();
        }

        public IMaterialViewModel Clone(IMaterialViewModel source)
        {
            throw new NotImplementedException();
        }

        public IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue)
        {
            if (primaryKeyValue == null)
            {
                return null;
            }

            // Todo: Add caching for this
            return
                this.projectContent.TypedLoad(
                    new ContentQuery<MetaDataEntry>().IsEqual("Id", primaryKeyValue)).ToList<MetaDataEntry>();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Unload()
        {
            this.materials.Clear();
        }

        private void InitializeProject(string rootPath)
        {
            string resourcePath = Path.Combine(rootPath, "Data");
            this.projectResources = this.engineFactory.GetResourceManager(resourcePath);

            string contentPath = Path.Combine(rootPath, "Main.db");
            this.projectContent = this.engineFactory.GetContentManager(this.projectResources, new ResourceLink { Path = contentPath });
        }

        private void NotifyProjectChanged()
        {
            var handler = this.ProjectChanged;
            if (handler != null)
            {
                handler();
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
