using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ICarbedSettings settings;

        private readonly List<IMaterialViewModel> materials;
        private readonly List<IFolderViewModel> folders;

        private string projectPath;
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
            this.settings = factory.Get<ICarbedSettings>();

            this.materials = new List<IMaterialViewModel>();
            this.folders = new List<IFolderViewModel>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event ProjectChangedEventHandler ProjectChanged;
        
        public bool IsProjectLoaded
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

        public IReadOnlyCollection<IFolderViewModel> Folders
        {
            get
            {
                return this.folders.AsReadOnly();
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

            this.CloseProject();
            this.settings.Save(Path.GetDirectoryName(file));

            // Move the database file over to our new location
            if (File.Exists(projectPath))
            {
                File.Copy(projectPath, file);
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

            ContentQueryResult<ResourceTree> treeData = this.projectContent.TypedLoad(new ContentQuery<ResourceTree>().IsEqual("Parent", null));
            var treeEntries = treeData.ToList<ResourceTree>();
            foreach (ResourceTree entry in treeEntries)
            {
                IFolderViewModel vm = this.viewModelFactory.GetFolderViewModel(entry);
                vm.Load();
                this.folders.Add(vm);
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
            material.Delete(this.projectContent);
        }

        public IMaterialViewModel Clone(IMaterialViewModel source)
        {
            throw new NotImplementedException();
        }

        public IFolderViewModel AddFolder()
        {
            var vm = this.viewModelFactory.GetFolderViewModel(new ResourceTree());
            this.folders.Add(vm);
            return vm;
        }

        public void Save(IFolderViewModel folder)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            folder.Save(this.projectContent, this.projectResources);
        }

        public void Delete(IFolderViewModel folder)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            folder.Delete(this.projectContent, this.projectResources);
            if (this.folders.Contains(folder))
            {
                this.folders.Remove(folder);
            }
        }

        public IFolderViewModel Clone(IFolderViewModel source)
        {
            throw new NotImplementedException();
        }
        
        public IResourceViewModel AddResource()
        {
            return this.viewModelFactory.GetResourceViewModel(new ResourceEntry());
        }

        public void Save(IResourceViewModel resource)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            resource.Save(this.projectContent, this.projectResources);
        }

        public void Delete(IResourceViewModel resource)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            resource.Delete(this.projectContent, this.projectResources);
        }

        public IResourceViewModel Clone(IResourceViewModel source)
        {
            throw new NotImplementedException();
        }

        public IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue, MetaDataTargetEnum target)
        {
            if (primaryKeyValue == null)
            {
                return null;
            }

            // Todo: Add caching for this
            return
                this.projectContent.TypedLoad(
                    new ContentQuery<MetaDataEntry>().IsEqual("TargetId", primaryKeyValue).IsEqual("Target", target)).ToList<MetaDataEntry>();
        }

        public IList<IFolderViewModel> GetResourceTreeChildren(int parent)
        {
            // Todo: Cache if this gets to slow
            IList<ResourceTree> treeData = this.projectContent.TypedLoad(new ContentQuery<ResourceTree>().IsEqual("Parent", parent)).ToList<ResourceTree>();
            return treeData.Select(x => this.viewModelFactory.GetFolderViewModel(x)).ToList();
        }

        public IList<IResourceViewModel> GetResourceTreeContent(int node)
        {
            // Todo: Cache if this gets to slow
            IList<ResourceEntry> resourceData =
                this.projectContent.TypedLoad(new ContentQuery<ResourceEntry>().IsEqual("TreeNode", node))
                    .ToList<ResourceEntry>();
            return resourceData.Select(x => this.viewModelFactory.GetResourceViewModel(x)).ToList();
        }

        public IFolderViewModel LocateFolder(string hash)
        {
            foreach (IFolderViewModel folder in folders)
            {
                var result = this.LocateFolder(folder, hash);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Unload()
        {
            this.materials.Clear();
            this.folders.Clear();
        }

        private void InitializeProject(string rootPath)
        {
            string resourcePath = Path.Combine(rootPath, "Data");
            this.projectResources = this.engineFactory.GetResourceManager(resourcePath);

            string contentPath = Path.Combine(rootPath, "Main.db");
            this.projectContent = this.engineFactory.GetContentManager(this.projectResources, contentPath);

            this.settings.Load(rootPath);
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

        private IFolderViewModel LocateFolder(IFolderViewModel current, string hash)
        {
            if (current.Hash.Equals(hash))
            {
                return current;
            }

            if (current.Content == null || current.Content.Count <= 0)
            {
                return null;
            }

            foreach (ICarbedDocument document in current.Content)
            {
                if (document as IFolderViewModel == null)
                {
                    continue;
                }

                var result = this.LocateFolder(document as IFolderViewModel, hash);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
