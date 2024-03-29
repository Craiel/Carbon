﻿namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.UtilsWPF;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;

    using GrandSeal.Editor.Contracts;

    using Microsoft.Win32;

    using global::GrandSeal.Editor.Views;

    public class FolderViewModel : ContentViewModel, IFolderViewModel
    {
        private readonly IEditorLogic logic;
        private readonly IFactory factory;
        private readonly ObservableCollection<IEditorDocument> content;
        private readonly IMainViewModel mainViewModel;
        private readonly ResourceTree data;

        private bool isExpanded;
        private bool isContentLoaded;
        
        private IFolderViewModel parent;

        private ICommand commandAddExistingResources;
        private ICommand commandAddFolder;
        private ICommand commandExpandAll;
        private ICommand commandCollapseAll;
        private ICommand commandCopyPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderViewModel(IFactory factory)
            : base(factory)
        {
            this.factory = factory;
            this.logic = factory.Resolve<IEditorLogic>();            
            this.mainViewModel = factory.Resolve<IMainViewModel>();

            this.content = new ObservableCollection<IEditorDocument>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.Name;
            }
        }

        public override bool IsChanged
        {
            get
            {
                return this.data.IsChanged;
            }
        }

        public bool IsContentLoaded
        {
            get
            {
                return this.isContentLoaded;
            }

            private set
            {
                if (this.isContentLoaded != value)
                {
                    this.isContentLoaded = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int? Id
        {
            get
            {
                return this.data.Id;
            }
        }

        public int? ContentCount
        {
            get
            {
                return this.GetMetaValueInt(MetaDataKey.ContentCount);
            }

            private set
            {
                if (this.GetMetaValueInt(MetaDataKey.ContentCount) != value)
                {
                    this.SetMetaValue(MetaDataKey.ContentCount, value);
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string Hash
        {
            get
            {
                return this.data.Hash;
            }
        }
        
        public CarbonDirectory FullPath
        {
            get
            {
                if (!this.IsNamed)
                {
                    System.Diagnostics.Trace.TraceWarning("FullPath queried with name not being set yet!");
                    return null;
                }

                string name = this.Name;
                if (this.parent != null)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return this.parent.FullPath;
                    }

                    return this.parent.FullPath.ToDirectory(name);
                }

                return new CarbonDirectory(name ?? string.Empty);
            }
        }

        public ReadOnlyObservableCollection<IEditorDocument> Content
        {
            get
            {
                return new ReadOnlyObservableCollection<IEditorDocument>(this.content);
            }
        }
        
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        
        public IFolderViewModel Parent 
        {
            get
            {
                return this.parent;
            }

            set
            {
                if (this.parent != value)
                {
                    this.parent = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ICommand CommandAddExistingResources
        {
            get
            {
                return this.commandAddExistingResources ??
                       (this.commandAddExistingResources = new RelayCommand(this.OnAddExistingResources));
            }
        }

        public ICommand CommandAddFolder
        {
            get
            {
                return this.commandAddFolder ?? (this.commandAddFolder = new RelayCommand(this.OnAddFolder));
            }
        }

        public ICommand CommandOpenNewDialog
        {
            get
            {
                return this.mainViewModel.CommandOpenNewDialog;
            }
        }

        public ICommand CommandExpandAll
        {
            get
            {
                return this.commandExpandAll ??
                       (this.commandExpandAll = new RelayCommand(this.OnExpandAll, this.CanExpandAll));
            }
        }

        public ICommand CommandCollapseAll
        {
            get
            {
                return this.commandCollapseAll ??
                       (this.commandCollapseAll = new RelayCommand(this.OnCollapseAll, this.CanCollapseAll));
            }
        }

        public ICommand CommandCopyPath
        {
            get
            {
                return this.commandCopyPath ?? (this.commandCopyPath = new RelayCommand(this.OnCopyPath));
            }
        }

        public void AddContent(IResourceViewModel newContent)
        {
            if (this.content.Contains(newContent))
            {
                throw new InvalidOperationException("Content was already added");
            }
            
            newContent.Parent = this;
            this.ContentCount++;
            this.content.Add(newContent);
            this.NotifyPropertyChanged("ContentCount");
        }

        public void RemoveContent(IResourceViewModel oldContent)
        {
            if (!this.content.Contains(oldContent))
            {
                throw new InvalidOperationException("Folder does not contain content to be removed");
            }

            this.ContentCount--;
            this.content.Remove(oldContent);
            this.NotifyPropertyChanged("ContentCount");
        }

        public void SetExpand(bool expanded)
        {
            this.IsExpanded = expanded;
            foreach (IEditorDocument child in this.content)
            {
                if ((child as IFolderViewModel) == null)
                {
                    continue;
                }

                ((IFolderViewModel)child).SetExpand(expanded);
            }
        }

        public IFolderViewModel AddFolder()
        {
            var vm = this.factory.Resolve<IFolderViewModel>();
            vm.Parent = this;
            this.content.Add(vm);
            return vm;
        }

        public void RemoveFolder(IFolderViewModel folder)
        {
            this.content.Remove(folder);
        }

        public override void Load()
        {
            base.Load();

            if (this.data.Id == null)
            {
                return;
            }

            this.content.Clear();
            IList<IFolderViewModel> children = this.logic.GetResourceTreeChildren((int)this.data.Id);
            TaskProgress.CurrentMaxProgress += children.Count;
            foreach (IFolderViewModel child in children)
            {
                TaskProgress.CurrentProgress++;
                TaskProgress.CurrentMessage = "Folder: " + child.Id.ToString();
                child.Load();
                child.Parent = this;
                this.content.Add(child);
            }

            int resourceCount = 0;
            IList<IResourceViewModel> contentList = this.logic.GetResourceTreeContent((int)this.data.Id);
            TaskProgress.CurrentMaxProgress += contentList.Count;
            foreach (IResourceViewModel entry in contentList)
            {
                TaskProgress.CurrentProgress++;
                TaskProgress.CurrentMessage = "Content: " + entry.Id.ToString();
                resourceCount++;
                entry.Parent = this;
                entry.Load();
                this.content.Add(entry);
            }

            // Check our content meta information and update if it is mismatching
            if (this.ContentCount != resourceCount)
            {
                if (resourceCount == 0)
                {
                    this.ContentCount = null;
                }
                else
                {
                    this.ContentCount = resourceCount;
                }                
            }
        }

        public void Save(IContentManager target, IResourceManager resourceTarget, bool force = false)
        {
            // Update our parent id information before saving
            if (this.parent != null && this.data.Parent != this.parent.Id)
            {
                this.data.Parent = this.parent.Id;
            }

            this.data.Hash = HashUtils.BuildResourceHash(this.FullPath.ToString());
            this.Save(target);
            
            // Save all our children as well
            TaskProgress.CurrentMaxProgress += this.content.Count;
            foreach (IEditorDocument entry in this.content)
            {
                TaskProgress.CurrentProgress++;
                if (entry as IFolderViewModel != null)
                {
                    TaskProgress.CurrentMessage = ((IFolderViewModel)entry).FullPath.ToString();
                    ((IFolderViewModel)entry).Save(target, resourceTarget, force);
                    continue;
                }

                if (entry as IResourceViewModel != null)
                {
                    TaskProgress.CurrentMessage = string.Format("{0} Resource: {1}", ((IResourceViewModel)entry).Type, entry.Name);
                    ((IResourceViewModel)entry).Save(target, resourceTarget, force);
                }
            }

            this.NotifyPropertyChanged();
        }

        public void Delete(IContentManager target, IResourceManager resourceTarget)
        {
            this.Delete(target);

            IList<IEditorDocument> deleteQueue = new List<IEditorDocument>(this.content);
            foreach (IEditorDocument entry in deleteQueue)
            {
                if (entry as IFolderViewModel != null)
                {
                    ((IFolderViewModel)entry).Delete(target, resourceTarget);
                    continue;
                }

                if (entry as IResourceViewModel != null)
                {
                    ((IResourceViewModel)entry).Delete(target, resourceTarget);
                }
            }

            if (this.parent != null)
            {
                this.parent.RemoveFolder(this);
            }

            this.NotifyPropertyChanged();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override object CreateMemento()
        {
            return this.data.Clone(fullCopy: true);
        }

        protected override void RestoreMemento(object memento)
        {
            var source = memento as ICarbonContent;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.CreateUndoState();
            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }

        protected override bool CanSave(bool force)
        {
            return this.content.Count > 0;
        }

        protected override void OnSave(bool force)
        {
            if (force)
            {
                new TaskProgress(new[] { new Task(() => this.logic.Save(this, true)) }, 1);
                return;
            }
            
            this.logic.Save(this);
        }

        protected override void OnDelete()
        {
            if (MessageBox.Show(
                "Delete Folder and all contents of " + this.Name,
                "Confirmation",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.logic.Delete(this);
        }

        protected override void OnRefresh()
        {
            IList<IEditorDocument> refreshList = new List<IEditorDocument>(this.content);
            foreach (IEditorDocument document in refreshList)
            {
                document.CommandRefresh.Execute(null);
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAddExistingResources()
        {
            var dialog = new OpenFileDialog { CheckFileExists = true, CheckPathExists = true, Multiselect = true };
            if (dialog.ShowDialog() == true)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    IResourceViewModel vm = this.GetResourceViewModelForFile(new CarbonFile(fileName));
                    vm.Parent = this;
                    vm.SelectFile(new CarbonFile(fileName));
                    this.content.Add(vm);
                }

                this.ContentCount += dialog.FileNames.Length;
            }
        }

        private IResourceViewModel GetResourceViewModelForFile(CarbonFile file)
        {
            switch (file.Extension)
            {
                case ".dds":
                case ".png":
                case ".tga":
                case ".tif":
                case ".jpg":
                    {
                        return this.logic.AddResourceTexture();
                    }

                case ".dae":
                    {
                        return this.logic.AddResourceModel();
                    }

                case ".xcd":
                    {
                        return this.logic.AddResourceStage();
                    }

                case ".lua":
                    {
                        return this.logic.AddResourceScript();
                    }

                case ".ttf":
                    {
                        return this.logic.AddResourceFont();
                    }

                case ".csaml":
                    {
                        return this.logic.AddResourceUserInterface();
                    }

                default:
                    {
                        return this.logic.AddResourceRaw();
                    }
            }
        }

        private void OnAddFolder()
        {
            this.AddFolder();
        }

        private void OnExpandAll()
        {
            this.SetExpand(true);
        }

        private bool CanExpandAll()
        {
            return this.content.Count > 0;
        }

        private void OnCollapseAll()
        {
            this.SetExpand(false);
        }

        private bool CanCollapseAll()
        {
            return this.content.Count > 0;
        }

        private void OnCopyPath()
        {
            Clipboard.SetText(this.FullPath.ToString());
        }
    }
}
