using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

using Core.Engine.Contracts;
using Core.Engine.Contracts.Resource;
using Core.Engine.Resource;
using Core.Engine.Resource.Content;
using Core.Utils.Contracts;
using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Views;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace GrandSeal.Editor.Logic
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Core.Utils.IO;

    using GrandSeal.Editor.Properties;

    public class EditorLogic : EditorBase, IEditorLogic
    {
        private const int RecentProjectMaximum = 10;

        private readonly IEngineFactory engineFactory;
        private readonly ILog log;
        private readonly IViewModelFactory viewModelFactory;
        private readonly IEditorSettings settings;

        private readonly ObservableCollection<IMaterialViewModel> materials;
        private readonly ObservableCollection<IFontViewModel> fonts;
        private readonly ObservableCollection<IFolderViewModel> folders;
        private readonly ObservableCollection<CarbonDirectory> recentProjects;

        private CarbonDirectory projectPath;
        private IContentManager projectContent;
        private IResourceManager projectResources;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EditorLogic(IEngineFactory factory)
        {
            this.engineFactory = factory;
            this.log = factory.Get<IEditorLog>().AquireContextLog("Logic");
            this.viewModelFactory = factory.Get<IViewModelFactory>();
            this.settings = factory.Get<IEditorSettings>();

            this.materials = new ObservableCollection<IMaterialViewModel>();
            this.fonts = new ObservableCollection<IFontViewModel>();
            this.folders = new ObservableCollection<IFolderViewModel>();
            this.recentProjects = new ObservableCollection<CarbonDirectory>();

            this.LoadSyntaxHightlighting();
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

        public CarbonDirectory ProjectLocation
        {
            get
            {
                return this.projectPath;
            }
        }

        public ReadOnlyObservableCollection<IMaterialViewModel> Materials
        {
            get
            {
                return new ReadOnlyObservableCollection<IMaterialViewModel>(this.materials);
            }
        }

        public ReadOnlyObservableCollection<IFontViewModel> Fonts
        {
            get
            {
                return new ReadOnlyObservableCollection<IFontViewModel>(this.fonts);
            }
        }

        public ReadOnlyObservableCollection<IFolderViewModel> Folders
        {
            get
            {
                return new ReadOnlyObservableCollection<IFolderViewModel>(this.folders);
            }
        }

        public ReadOnlyObservableCollection<CarbonDirectory> RecentProjects
        {
            get
            {
                return new ReadOnlyObservableCollection<CarbonDirectory>(this.recentProjects);
            }
        }

        public void NewProject()
        {
            this.CloseProject();
            this.projectPath = CarbonDirectory.GetTempDirectory();
            this.InitializeProject(this.projectPath);
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

            this.projectPath = null;
            this.NotifyProjectChanged();
        }

        public void OpenProject(CarbonDirectory path)
        {
            this.CloseProject();

            if (path.IsNull)
            {
                throw new ArgumentException("Invalid Path specified: " + path);
            }

            this.projectPath = path;
            this.InitializeProject(path);
            this.Reload();
            this.CheckProjectDefaults();
            this.AddToRecentProjects(path);
            this.NotifyProjectChanged();
        }

        public void SaveProject(CarbonDirectory path)
        {
            this.log.Warning("Saving into different file is not fully supported yet!");
            /*
             * Todo:
             * - Choose a target path instead of file
             * - Move the Resources from the temporary folder as well
             * - Cleanup the temporary data
             */

            this.CloseProject();
            this.settings.Save(path);

            // Move the database file over to our new location
            if (this.projectPath.Exists)
            {
                this.projectPath.CopyTo(path);
            }
            
            this.OpenProject(path);
        }

        public void Reload()
        {
            this.Unload();

            // Resources need to be first, content has dependencies
            Task[] tasks = new[] { new Task(this.ReloadResources) };
            TaskProgress.Message = "Loading Resources";
            new TaskProgress(tasks);

            tasks = new[] { new Task(this.ReloadMaterials), new Task(this.ReloadFonts) };
            TaskProgress.Message = "Loading Content";
            new TaskProgress(tasks, tasks.Length);
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

        public IFontViewModel AddFont()
        {
            var vm = this.viewModelFactory.GetFontViewModel(new FontEntry());
            this.fonts.Insert(0, vm);
            return vm;
        }

        public void Save(IFontViewModel font)
        {
            if (this.projectContent == null)
            {
                throw new InvalidOperationException();
            }

            font.Save(this.projectContent);
        }

        public void Delete(IFontViewModel font)
        {
            font.Delete(this.projectContent);
        }

        public IFontViewModel Clone(IFontViewModel source)
        {
            throw new NotImplementedException();
        }

        public IFolderViewModel AddFolder()
        {
            var vm = this.viewModelFactory.GetFolderViewModel(new ResourceTree());
            this.folders.Add(vm);
            return vm;
        }

        public void Save(IFolderViewModel folder, bool force = false)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            folder.Save(this.projectContent, this.projectResources, force);
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
        
        public IResourceTextureViewModel AddResourceTexture()
        {
            return this.viewModelFactory.GetResourceTextureViewModel(new ResourceEntry { Type = ResourceType.Texture });
        }

        public IResourceModelViewModel AddResourceModel()
        {
            return this.viewModelFactory.GetResourceModelViewModel(new ResourceEntry { Type = ResourceType.Model });
        }

        public IResourceScriptViewModel AddResourceScript()
        {
            return this.viewModelFactory.GetResourceScriptViewModel(new ResourceEntry { Type = ResourceType.Script });
        }

        public IResourceFontViewModel AddResourceFont()
        {
            return this.viewModelFactory.GetResourceFontViewModel(new ResourceEntry { Type = ResourceType.Font });
        }

        public IResourceRawViewModel AddResourceRaw()
        {
            return this.viewModelFactory.GetResourceRawViewModel(new ResourceEntry { Type = ResourceType.Raw });
        }

        public IResourceStageViewModel AddResourceStage()
        {
            return this.viewModelFactory.GetResourceStageViewModel(new ResourceEntry { Type = ResourceType.Stage });
        }

        public IResourceUserInterfaceViewModel AddResourceUserInterface()
        {
            return this.viewModelFactory.GetResourceUserInterfaceViewModel(new ResourceEntry { Type = ResourceType.UserInterface });
        }

        public void Save(IResourceViewModel resource)
        {
            if (this.projectContent == null || this.projectResources == null)
            {
                throw new InvalidOperationException();
            }

            resource.Save(this.projectContent, this.projectResources, false);
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
                    new ContentQuery<MetaDataEntry>().IsEqual("TargetId", primaryKeyValue).IsEqual("Target", target)).ToList();
        }

        public IList<IFolderViewModel> GetResourceTreeChildren(int parent)
        {
            // Todo: Cache if this gets to slow
            IList<ResourceTree> treeData = this.projectContent.TypedLoad(new ContentQuery<ResourceTree>().IsEqual("Parent", parent)).ToList();
            return treeData.Select(x => this.viewModelFactory.GetFolderViewModel(x)).ToList();
        }

        public IList<IResourceViewModel> GetResourceTreeContent(int node)
        {
            // Todo: Cache if this gets to slow
            IList<ResourceEntry> resourceData =
                this.projectContent.TypedLoad(new ContentQuery<ResourceEntry>().IsEqual("TreeNode", node))
                    .ToList();

            IList<IResourceViewModel> results = new List<IResourceViewModel>();
            foreach (ResourceEntry entry in resourceData)
            {
                switch (entry.Type)
                {
                    case ResourceType.Texture:
                        {
                            results.Add(this.viewModelFactory.GetResourceTextureViewModel(entry));
                            break;
                        }
                    case ResourceType.Model:
                        {
                            results.Add(this.viewModelFactory.GetResourceModelViewModel(entry));
                            break;
                        }

                    case ResourceType.Script:
                        {
                            results.Add(this.viewModelFactory.GetResourceScriptViewModel(entry));
                            break;
                        }

                    case ResourceType.Font:
                        {
                            results.Add(this.viewModelFactory.GetResourceFontViewModel(entry));
                            break;
                        }

                    case ResourceType.Raw:
                        {
                            results.Add(this.viewModelFactory.GetResourceRawViewModel(entry));
                            break;
                        }

                    case ResourceType.Stage:
                        {
                            results.Add(this.viewModelFactory.GetResourceStageViewModel(entry));
                            break;
                        }

                    case ResourceType.UserInterface:
                        {
                            results.Add(this.viewModelFactory.GetResourceUserInterfaceViewModel(entry));
                            break;
                        }

                    default:
                        {
                            throw new InvalidDataException("Unknown resource type " + entry.Type);
                        }
                }
            }

            return results;
        }

        public IFolderViewModel LocateFolder(string hash)
        {
            foreach (IFolderViewModel folder in this.folders)
            {
                var result = this.LocateFolder(folder, hash);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IResourceViewModel LocateResource(int id)
        {
            foreach (IFolderViewModel folder in this.folders)
            {
                var result = this.LocateResource(folder, id);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IResourceViewModel LocateResource(string hash)
        {
            foreach (IFolderViewModel folder in this.folders)
            {
                var result = this.LocateResource(folder, hash);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IList<IResourceViewModel> LocateResources(string filter)
        {
            IList<IResourceViewModel> results = new List<IResourceViewModel>();
            foreach (IFolderViewModel folder in this.folders)
            {
                this.LocateResources(results, folder, filter);
            }

            return results;
        }

        public void ReloadSettings()
        {
            Settings.Default.Reload();
            this.recentProjects.Clear();
            if (Settings.Default.ProjectHistory != null)
            {
                foreach (string history in Settings.Default.ProjectHistory)
                {
                    this.recentProjects.Add(new CarbonDirectory(history));
                }
            }
        }

        public void SaveSettings()
        {
            var history = new StringCollection();
            foreach (CarbonPath path in this.recentProjects)
            {
                history.Add(path.ToString());
            }

            Settings.Default.ProjectHistory = history;
            Settings.Default.Save();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ReloadResources()
        {
            TaskProgress.CurrentProgress = 0;
            TaskProgress.CurrentMaxProgress = 0;
            ContentQueryResult<ResourceTree> treeData = this.projectContent.TypedLoad(new ContentQuery<ResourceTree>().IsEqual("Parent", null));
            var treeEntries = treeData.ToList();
            TaskProgress.CurrentMaxProgress = treeEntries.Count;
            foreach (ResourceTree entry in treeEntries)
            {
                TaskProgress.CurrentProgress++;
                TaskProgress.CurrentMessage = "Folder: " + entry.Id.ToString();
                IFolderViewModel vm = this.viewModelFactory.GetFolderViewModel(entry);
                vm.Load();
                Application.Current.Dispatcher.Invoke(() => this.folders.Add(vm));
            }
        }

        private void ReloadMaterials()
        {
            ContentQueryResult<MaterialEntry> materialData = this.projectContent.TypedLoad(new ContentQuery<MaterialEntry>());
            var materialEntries = materialData.ToList();
            TaskProgress.CurrentMaxProgress += materialEntries.Count;
            foreach (MaterialEntry entry in materialEntries)
            {
                TaskProgress.CurrentProgress++;
                IMaterialViewModel vm = this.viewModelFactory.GetMaterialViewModel(entry);
                vm.Load();
                Application.Current.Dispatcher.Invoke(() => this.materials.Add(vm));
            }
        }

        private void ReloadFonts()
        {
            ContentQueryResult<FontEntry> fontData = this.projectContent.TypedLoad(new ContentQuery<FontEntry>());
            var fontEntries = fontData.ToList();
            TaskProgress.CurrentMaxProgress += fontEntries.Count;
            foreach (FontEntry entry in fontEntries)
            {
                TaskProgress.CurrentProgress++;
                IFontViewModel vm = this.viewModelFactory.GetFontViewModel(entry);
                vm.Load();
                Application.Current.Dispatcher.Invoke(() => this.fonts.Add(vm));
            }
        }

        private void Unload()
        {
            this.materials.Clear();
            this.folders.Clear();
        }

        private void InitializeProject(CarbonDirectory rootPath)
        {
            CarbonDirectory resourcePath = rootPath.ToDirectory("Data");
            this.projectResources = this.engineFactory.GetResourceManager(resourcePath);

            CarbonFile contentFile = rootPath.ToFile("Main.db");
            this.projectContent = this.engineFactory.GetContentManager(this.projectResources, contentFile);

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

        private void AddToRecentProjects(CarbonDirectory newPath)
        {
            // Update the recent project list, move to top if its already in there
            if (this.recentProjects.Contains(newPath))
            {
                int index = this.recentProjects.IndexOf(newPath);
                if (index > 0)
                {
                    this.recentProjects.Move(index, 0);
                }
            }
            else
            {
                this.recentProjects.Add(newPath);   
            }

            while (this.recentProjects.Count > RecentProjectMaximum)
            {
                this.recentProjects.RemoveAt(this.recentProjects.Count - 1);
            }
        }

        private IFolderViewModel LocateFolder(IFolderViewModel current, string hash)
        {
            if (!string.IsNullOrEmpty(current.Hash) && current.Hash.Equals(hash))
            {
                return current;
            }

            if (current.Content == null || current.Content.Count <= 0)
            {
                return null;
            }

            foreach (IEditorDocument document in current.Content)
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

        private IResourceViewModel LocateResource(IFolderViewModel current, string hash)
        {
            if (current.Content == null || current.Content.Count <= 0)
            {
                return null;
            }

            foreach (IEditorDocument document in current.Content)
            {
                if (document as IResourceViewModel != null)
                {
                    if (((IResourceViewModel)document).Hash.Equals(hash, StringComparison.Ordinal))
                    {
                        return document as IResourceViewModel;
                    }

                    continue;
                }

                if (document as IFolderViewModel != null)
                {
                    var result = this.LocateResource(document as IFolderViewModel, hash);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private IResourceViewModel LocateResource(IFolderViewModel current, int id)
        {
            if (current.Content == null || current.Content.Count <= 0)
            {
                return null;
            }

            foreach (IEditorDocument document in current.Content)
            {
                if (document as IResourceViewModel != null)
                {
                    if (((IResourceViewModel)document).Id == id)
                    {
                        return document as IResourceViewModel;
                    }

                    continue;
                }

                if (document as IFolderViewModel != null)
                {
                    var result = this.LocateResource(document as IFolderViewModel, id);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void LocateResources(IList<IResourceViewModel> target, IFolderViewModel current, string filter)
        {
            if (current.Content == null || current.Content.Count <= 0)
            {
                return;
            }

            foreach (IEditorDocument document in current.Content)
            {
                if (document as IResourceViewModel != null)
                {
                    if (string.IsNullOrEmpty(filter) || document.Name.ToLower().Contains(filter))
                    {
                        target.Add(document as IResourceViewModel);
                    }

                    continue;
                }

                if (document as IFolderViewModel != null)
                {
                    this.LocateResources(target, document as IFolderViewModel, filter);
                }
            }
        }

        // This will check and add / set some default data for the project
        private void CheckProjectDefaults()
        {
            if (this.folders.Count <= 0)
            {
                this.AddFolder().Name = "Textures";
                this.AddFolder().Name = "Models";
                this.AddFolder().Name = "Scripts";
                this.AddFolder().Name = "Fonts";
                this.AddFolder().Name = "UserInterface";
            }
        }

        private void LoadSyntaxHightlighting()
        {
            using (Stream s = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(@"GrandSeal.Editor.Resources.Lua.xshd"))
            {
                if (s != null)
                {
                    using (var reader = new XmlTextReader(s))
                    {
                        IHighlightingDefinition luaHighlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        if (luaHighlightingDefinition != null)
                        {
                            HighlightingManager.Instance.RegisterHighlighting(
                                "Lua", new[] { ".lua" }, luaHighlightingDefinition);
                        }
                    }
                }
            }
        }
    }
}
