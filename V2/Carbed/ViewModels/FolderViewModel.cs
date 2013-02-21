using System;
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
    public class FolderViewModel : DocumentViewModel, IFolderViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;
        private readonly ObservableCollection<ICarbedDocument> content;
        private readonly IMainViewModel mainViewModel;
        private readonly ResourceTree data;

        private bool isExpanded;

        private IFolderViewModel parent;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderViewModel(IEngineFactory factory, ResourceTree data)
            : base(factory)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.viewModelFactory = factory.Get<IViewModelFactory>();
            this.mainViewModel = factory.Get<IMainViewModel>();
            this.data = data;

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

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.data.Name))
                {
                    return "<no name>";
                }

                return this.data.Name;
            }
            set
            {
                if (this.data.Name != value)
                {
                    this.CreateUndoState();
                    this.data.Name = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("IsChanged");
                }
            }
        }

        public string FullPath
        {
            get
            {
                string name = this.data.Name;
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
            this.content.Remove(folder);
        }

        public override void Load()
        {
            base.Load();

            if (this.data.Id == null)
            {
                return;
            }

            this.logic.GetResourceTreeChildren((int)this.data.Id);
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
            ICarbonContent source = memento as ICarbonContent;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.CreateUndoState();
            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAddFolder(object obj)
        {
            var vm = this.viewModelFactory.GetFolderViewModel(new ResourceTree());
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
            this.parent.DeleteFolder(this);
        }

        private bool CanDeleteFolder(object obj)
        {
            return true;
        }
    }
}
