using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils;

namespace Carbed.ViewModels
{
    public class FolderViewModel : ContentViewModel, IFolderViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;
        private readonly ObservableCollection<ICarbedDocument> content;
        private readonly IMainViewModel mainViewModel;
        private readonly ResourceTree data;

        private bool isExpanded;
        private bool isContentLoaded;

        private MetaDataEntry contentCount;

        private IFolderViewModel parent;

        private ICommand commandAddFolder;
        private ICommand commandDeleteFolder;
        private ICommand commandExpandAll;
        private ICommand commandCollapseAll;
        private ICommand commandCopyPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderViewModel(IEngineFactory factory, ResourceTree data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.viewModelFactory = factory.Get<IViewModelFactory>();
            this.mainViewModel = factory.Get<IMainViewModel>();
            this.data = data;

            this.content = new ObservableCollection<ICarbedDocument>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
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
                if (this.contentCount == null)
                {
                    return null;
                }

                return this.contentCount.ValueInt;
            }
        }
        
        public string FullPath
        {
            get
            {
                if (!this.IsNamed)
                {
                    this.Log.Warning("FullPath queried with name not being set yet!");
                    return string.Empty;
                }

                string name = this.Name;
                if (this.parent != null)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return this.parent.FullPath;
                    }

                    return Path.Combine(this.parent.FullPath, name);
                }

                return name ?? string.Empty;
            }
        }

        public ReadOnlyObservableCollection<ICarbedDocument> Content
        {
            get
            {
                return new ReadOnlyObservableCollection<ICarbedDocument>(this.content);
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

        public ICommand CommandAddFolder
        {
            get
            {
                return this.commandAddFolder ?? (this.commandAddFolder = new RelayCommand(this.OnAddFolder));
            }
        }

        public ICommand CommandDeleteFolder
        {
            get
            {
                return this.commandDeleteFolder ??
                       (this.commandDeleteFolder = new RelayCommand(this.OnDeleteFolder, this.CanDeleteFolder));
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

            if (this.contentCount == null)
            {
                this.contentCount = new MetaDataEntry { Key = MetaDataKey.ContentCount, ValueInt = 0 };
            }

            newContent.Parent = this;
            this.contentCount.ValueInt++;
            this.content.Add(newContent);
            this.NotifyPropertyChanged("ContentCount");
        }

        public void RemoveContent(IResourceViewModel oldContent)
        {
            if (!this.content.Contains(oldContent))
            {
                throw new InvalidOperationException("Folder does not contain content to be removed");
            }

            if (this.contentCount == null)
            {
                throw new InvalidOperationException("Removing content while Content Count was null, this should not be happening");
            }

            this.contentCount.ValueInt--;
            this.content.Remove(oldContent);
            this.NotifyPropertyChanged("ContentCount");
        }

        public void SetExpand(bool expanded)
        {
            this.IsExpanded = expanded;
            foreach (ICarbedDocument child in this.content)
            {
                if ((child as IFolderViewModel) == null)
                {
                    continue;
                }

                ((IFolderViewModel)child).SetExpand(expanded);
            }
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
            foreach (IFolderViewModel child in children)
            {
                child.Load();
                this.content.Add(child);
            }

            IList<IResourceViewModel> contentList = this.logic.GetResourceTreeContent((int)this.data.Id);
            foreach (IResourceViewModel entry in contentList)
            {
                entry.Load();
                this.content.Add(entry);
            }
        }

        public void Save(IContentManager target, IResourceManager resourceTarget)
        {
            // Update our parent id information before saving
            if (this.parent != null && this.data.Parent != this.parent.Id)
            {
                this.data.Parent = this.parent.Id;
            }

            this.data.Hash = HashUtils.BuildResourceHash(this.FullPath);
            this.Save(target);

            if (this.contentCount != null)
            {
                target.Save(this.contentCount);
            }

            // Save all our children as well
            foreach (ICarbedDocument entry in this.content)
            {
                if (entry as IFolderViewModel != null)
                {
                    ((IFolderViewModel)entry).Save(target, resourceTarget);
                    continue;
                }

                if (entry as IResourceViewModel != null)
                {
                    ((IResourceViewModel)entry).Save(target, resourceTarget);
                }
            }

            this.NotifyPropertyChanged();
        }

        public void Delete(IContentManager target, IResourceManager resourceTarget)
        {
            this.Delete(target);

            IList<ICarbedDocument> deleteQueue = new List<ICarbedDocument>(this.content);
            foreach (ICarbedDocument entry in deleteQueue)
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

        protected override void OnSave(object obj)
        {
            this.logic.Save(this);
        }

        protected override void OnDelete(object arg)
        {
            this.logic.Delete(this);
        }

        protected override void LoadMetadata(IList<MetaDataEntry> metaData)
        {
            base.LoadMetadata(metaData);

            for (int i = 0; i < metaData.Count; i++)
            {
                if (metaData[i].Key == MetaDataKey.ContentCount)
                {
                    this.contentCount = metaData[i];
                    break;
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAddFolder(object obj)
        {
            var vm = this.viewModelFactory.GetFolderViewModel(new ResourceTree());
            vm.Parent = this;
            this.content.Add(vm);
        }

        private void OnExpandAll(object obj)
        {
            this.SetExpand(true);
        }

        private bool CanExpandAll(object obj)
        {
            return this.content.Count > 0;
        }

        private void OnCollapseAll(object obj)
        {
            this.SetExpand(false);
        }

        private bool CanCollapseAll(object obj)
        {
            return this.content.Count > 0;
        }

        private void OnDeleteFolder(object obj)
        {
            this.logic.Delete(this);
        }

        private bool CanDeleteFolder(object obj)
        {
            return true;
        }

        private void OnCopyPath(object obj)
        {
            Clipboard.SetText(this.FullPath);
        }
    }
}
