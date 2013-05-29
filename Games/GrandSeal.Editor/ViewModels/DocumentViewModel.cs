using System;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic;
using GrandSeal.Editor.Logic.MVVM;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.ViewModels
{
    public abstract class DocumentViewModel : EditorBase, IEditorDocument
    {
        private readonly IPropertyViewModel propertyViewModel;
        private readonly IMainViewModel mainViewModel;
        private readonly IUndoRedoManager undoRedoManager;

        private ICommand commandOpen;
        private ICommand commandSave;
        private ICommand commandClose;
        private ICommand commandDelete;
        private ICommand commandRefresh;
        
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
        public abstract string Title { get; }
        public abstract string Name { get; set; }

        public string ContentId { get; protected set; }

        public virtual bool IsNamed
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsChanged
        {
            get
            {
                return false;
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

        public ICommand CommandSave
        {
            get
            {
                return this.commandSave ?? (this.commandSave = new RelayCommand(this.OnSave, this.CanSave));
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

        public ICommand CommandRefresh
        {
            get
            {
                return this.commandRefresh ?? (this.commandRefresh = new RelayCommand(this.OnRefresh));
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
            return true;
        }

        protected virtual void OnSave(object obj)
        {
        }

        protected virtual bool CanSave(object obj)
        {
            return this.IsChanged;
        }

        protected virtual void OnClose(object arg)
        {
            this.mainViewModel.CloseDocument(this);
        }

        protected virtual bool CanClose(object arg)
        {
            return true;
        }

        protected virtual void OnDelete(object arg)
        {
        }

        protected virtual void OnRefresh(object arg)
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
