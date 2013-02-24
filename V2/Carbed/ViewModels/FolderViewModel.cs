using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class FolderViewModel : ContentViewModel, IFolderViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;
        private readonly ObservableCollection<IFolderViewModel> folders;
        private readonly ObservableCollection<ICarbedDocument> content;
        private readonly IMainViewModel mainViewModel;
        private readonly ResourceTree data;

        private bool isExpanded;

        private IFolderViewModel parent;

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

            this.folders = new ObservableCollection<IFolderViewModel>();
            this.content = new ObservableCollection<ICarbedDocument>();

            this.CommandAddFolder = new RelayCommand(this.OnAddFolder);
            this.CommandDeleteFolder = new RelayCommand(this.OnDeleteFolder, this.CanDeleteFolder);
            this.CommandExpandAll = new RelayCommand(this.OnExpandAll, this.CanExpandAll);
            this.CommandCollapseAll = new RelayCommand(this.OnCollapseAll, this.CanCollapseAll);
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

        public int? Id
        {
            get
            {
                return this.data.Id;
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

        public ReadOnlyObservableCollection<IFolderViewModel> Folders
        {
            get
            {
                return new ReadOnlyObservableCollection<IFolderViewModel>(this.folders);
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

        public ICommand CommandAddFolder { get; private set; }

        public ICommand CommandDeleteFolder { get; private set; }

        public ICommand CommandOpenNewDialog
        {
            get
            {
                return this.mainViewModel.CommandOpenNewDialog;
            }
        }

        public ICommand CommandExpandAll { get; private set; }

        public ICommand CommandCollapseAll { get; private set; }

        public void AddContent(IResourceViewModel newContent)
        {
            if (this.content.Contains(newContent))
            {
                throw new InvalidOperationException("Content was already added");
            }

            this.content.Add(newContent);
        }

        public void DeleteContent(IResourceViewModel oldContent)
        {
            if (!this.content.Contains(oldContent))
            {
                throw new InvalidOperationException("Folder does not contain content to be removed");
            }

            this.content.Remove(oldContent);
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

        public void DeleteFolder(IFolderViewModel folder)
        {
            this.folders.Remove(folder);
        }

        public override void Load()
        {
            base.Load();

            if (this.data.Id == null)
            {
                return;
            }

            this.folders.Clear();
            IList<IFolderViewModel> children = this.logic.GetResourceTreeChildren((int)this.data.Id);
            foreach (IFolderViewModel child in children)
            {
                this.folders.Add(child);
            }
        }

        public new void Save(IContentManager target)
        {
            // Update our parent id information before saving
            if (this.parent != null && this.data.Parent != this.parent.Id)
            {
                this.data.Parent = this.parent.Id;
            }

            base.Save(target);

            // Save all our children as well
            foreach (IFolderViewModel folder in this.Folders)
            {
                folder.Save(target);
            }

            this.NotifyPropertyChanged();
        }

        public new void Delete(IContentManager target)
        {
            base.Delete(target);

            foreach (IFolderViewModel folder in this.Folders)
            {
                folder.Delete(target);
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
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAddFolder(object obj)
        {
            var vm = this.viewModelFactory.GetFolderViewModel(new ResourceTree());
            vm.Parent = this;
            this.folders.Add(vm);
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
            if (this.parent != null)
            {
                this.parent.DeleteFolder(this);
            }
        }

        private bool CanDeleteFolder(object obj)
        {
            return true;
        }
    }
}
