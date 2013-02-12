using System;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public abstract class DocumentViewModel : CarbedBase, ICarbedDocument
    {
        private readonly IPropertyViewModel propertyViewModel;
        private readonly IMainViewModel mainViewModel;
        private readonly IUndoRedoManager undoRedoManager;

        private ICommand commandOpen;
        private ICommand commandClose;
        private ICommand commandDelete;
        
        private bool isSelected;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DocumentViewModel(IEngineFactory engineFactory)
        {
            this.propertyViewModel = engineFactory.Get<IPropertyViewModel>();
            this.mainViewModel = engineFactory.Get<IMainViewModel>();
            this.undoRedoManager = engineFactory.Get<IUndoRedoManager>();

            this.undoRedoManager.RegisterGroup(this);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Name { get; set; }

        public abstract string Title { get; }

        public bool HasName
        {
            get
            {
                return string.IsNullOrEmpty(this.Name);
            }
        }
        
        public virtual Uri IconUri
        {
            get
            {
                if (this.Template != null)
                {
                    return this.Template.IconUri;
                }

                return null;
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

        public ICommand CommandOpen
        {
            get
            {
                return this.commandOpen ?? (this.commandOpen = new RelayCommand(this.OnOpen, this.CanOpen));
            }
        }

        public ICommand CommandClose
        {
            get
            {
                return this.commandClose ?? (this.commandClose = new RelayCommand(this.OnClose, this.CanClose));
            }
        }

        public ICommand CommandDelete
        {
            get
            {
                return this.commandDelete ?? (this.commandDelete = new RelayCommand(this.OnDelete));
            }
        }

        public virtual void Load()
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected IDocumentTemplate Template { get; set; }

        protected virtual void OnOpen(object arg)
        {
            this.mainViewModel.OpenDocument(this);
        }

        protected virtual bool CanOpen(object arg)
        {
            // Todo
            return true;
        }

        protected virtual void OnClose(object arg)
        {
            this.mainViewModel.CloseDocument(this);
        }

        protected virtual bool CanClose(object arg)
        {
            // Todo
            return true;
        }

        protected virtual void OnDelete(object arg)
        {
        }

        protected void CreateUndoState()
        {
            var memento = this.CreateMemento();
            this.undoRedoManager.AddOperation(() => this.RestoreMemento(memento), string.Format("{0} Change", this.GetType().Name));
        }

        protected abstract object CreateMemento();
        protected abstract void RestoreMemento(object memento);
    }
}
