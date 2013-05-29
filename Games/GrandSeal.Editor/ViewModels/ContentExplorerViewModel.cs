using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.ViewModels
{
    public abstract class ContentExplorerViewModel<T> : ToolViewModel, IContentExplorerViewModel<T>
        where T : IEditorDocument
    {
        private readonly IEditorLogic logic;
        private readonly IEngineFactory factory;

        private readonly ObservableCollection<T> filteredDocuments;

        private ICommand commandSave;
        private ICommand commandReload;

        private IMainViewModel mainViewModel;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ContentExplorerViewModel(IEngineFactory factory, IEditorLogic logic)
        {
            this.factory = factory;
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;

            this.filteredDocuments = new ObservableCollection<T>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return "Content Explorer: " + typeof(T);
            }
        }
        
        public ReadOnlyObservableCollection<T> Documents
        {
            get
            {
                return new ReadOnlyObservableCollection<T>(this.filteredDocuments);
            }
        }

        public ICommand CommandOpenNewDialog
        {
            get
            {
                // Has to be acquired on demand otherwise we get circular IoC dependency
                if (this.mainViewModel == null)
                {
                    this.mainViewModel = this.factory.Get<IMainViewModel>();
                }

                return this.mainViewModel.CommandOpenNewDialog;
            }
        }
       
        public ICommand CommandSave
        {
            get
            {
                return this.commandSave ?? (this.commandSave = new RelayCommand(this.OnSave, this.CanSave));
            }
        }

        public ICommand CommandReload
        {
            get
            {
                return this.commandReload ?? (this.commandReload = new RelayCommand(this.OnReload, this.CanReload));
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void DoUpdate(ObservableCollection<T> target);

        protected void UpdateDocuments()
        {
            this.filteredDocuments.Clear();

            if (!this.logic.IsProjectLoaded)
            {
                return;
            }

            this.DoUpdate(this.filteredDocuments);
            this.NotifyPropertyChanged("Documents");
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnProjectChanged()
        {
            this.UpdateDocuments();
            this.NotifyPropertyChanged(string.Empty);
        }

        private void OnSave(object obj)
        {
            foreach (var document in this.Documents)
            {
                if (document.CommandSave.CanExecute(obj))
                {
                    document.CommandSave.Execute(obj);
                }
            }
        }

        private bool CanSave(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnReload(object obj)
        {
            this.UpdateDocuments();
        }

        private bool CanReload(object obj)
        {
            return this.logic.IsProjectLoaded;
        }
    }
}
