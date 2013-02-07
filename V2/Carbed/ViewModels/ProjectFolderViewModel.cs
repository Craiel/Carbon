using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic;
using Carbed.Logic.MVVM;

using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public class ProjectFolderViewModel : CarbedBase, IProjectFolderViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;
        private readonly ObservableCollection<IProjectFolderContent> content;
        private readonly SourceProjectFolder data;
        private readonly IPropertyViewModel propertyViewModel;
        private readonly IMainViewModel mainViewModel;
        private readonly IUndoRedoManager undoRedoManager;

        private bool isSelected;
        private bool isExpanded;

        private IProjectFolderViewModel parent;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProjectFolderViewModel(IEngineFactory factory, SourceProjectFolder data)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.viewModelFactory = factory.Get<IViewModelFactory>();
            this.propertyViewModel = factory.Get<IPropertyViewModel>();
            this.mainViewModel = factory.Get<IMainViewModel>();
            this.undoRedoManager = factory.Get<IUndoRedoManager>();
            this.data = data;
            this.content = new ObservableCollection<IProjectFolderContent>();

            this.CommandAddFolder = new RelayCommand(this.OnAddFolder, this.CanAddFolder);
            this.CommandDeleteFolder = new RelayCommand(this.OnDeleteFolder, this.CanDeleteFolder);
            this.CommandExpandAll = new RelayCommand(this.OnExpandAll, this.CanExpandAll);
            this.CommandCollapseAll = new RelayCommand(this.OnCollapseAll, this.CanCollapseAll);

            this.undoRedoManager.RegisterGroup(this);

            this.CreateViewModels();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HasName
        {
            get
            {
                return string.IsNullOrEmpty(this.data.Name);
            }
        }

        public string Name
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
                    this.NotifyPropertyChanged("HasName");
                }
            }
        }

        public ReadOnlyObservableCollection<IProjectFolderContent> Content
        {
            get
            {
                return new ReadOnlyObservableCollection<IProjectFolderContent>(this.content);
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.propertyViewModel.SetActivation(this, value);
                    this.undoRedoManager.ActivateGroup(this);
                    this.NotifyPropertyChanged();
                }
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

        public SourceFolderContent Data
        {
            get
            {
                return this.data;
            }
        }

        public IProjectFolderViewModel Parent 
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

        public void AddContent(IProjectFolderContent newContent)
        {
            if (this.content.Contains(newContent))
            {
                throw new InvalidOperationException("Content was already added");
            }

            this.data.Contents.Add(newContent.Data);
            this.content.Add(newContent);
        }

        public void DeleteContent(IProjectFolderContent oldContent)
        {
            if (!this.content.Contains(oldContent))
            {
                throw new InvalidOperationException("Folder does not contain content to be removed");
            }

            this.data.Contents.Remove(oldContent.Data);
            this.content.Remove(oldContent);
        }

        public void SetExpand(bool expanded)
        {
            this.IsExpanded = expanded;
            foreach (IProjectFolderContent child in this.content)
            {
                var vm = child as IProjectFolderViewModel;
                if (vm != null)
                {
                    vm.SetExpand(expanded);
                }
            }
        }

        

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAddFolder(object obj)
        {
            var child = new SourceProjectFolder();
            this.data.Contents.Add(child);
            var vm = this.viewModelFactory.GetFolderViewModel(child);
            this.content.Add(vm);
        }

        private bool CanAddFolder(object obj)
        {
            return this.logic.Project != null;
        }

        private void ClearViewModels()
        {
            this.content.Clear();
        }

        private void CreateViewModels()
        {
            this.ClearViewModels();
            foreach (SourceFolderContent child in this.data.Contents)
            {
                var folder = child as SourceProjectFolder;
                if (folder != null)
                {
                    var vm = this.viewModelFactory.GetFolderViewModel(folder);
                    vm.Parent = this;
                    this.content.Add(vm);
                    continue;
                }

                var mesh = child as SourceModel;
                if (mesh != null)
                {
                    var vm = this.viewModelFactory.GetModelViewModel(mesh);
                    vm.Parent = this;
                    this.content.Add(vm);
                    continue;
                }
            }
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
            this.parent.DeleteContent(this);
        }

        private bool CanDeleteFolder(object obj)
        {
            return true;
        }

        private void CreateUndoState()
        {
            var memento = this.CreateMemento();
            this.undoRedoManager.AddOperation(() => this.RestoreMemento(memento), "FolderChange");
        }

        private object CreateMemento()
        {
            return this.data.Clone();
        }

        private void RestoreMemento(object memento)
        {
            SourceProjectFolder source = memento as SourceProjectFolder;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.CreateUndoState();
            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
