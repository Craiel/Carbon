using System;
using System.Windows;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public abstract class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private readonly ResourceEntry data;

        private ICommand commandSelectFile;

        private IFolderViewModel parent;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.data = data;
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
                    this.MoveFile(value);
                }
            }
        }
        
        public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
        }

        public new void Save(IContentManager target)
        {
            base.Save(target);

            this.NotifyPropertyChanged();
        }

        public new void Delete(IContentManager target)
        {
            base.Delete(target);
            
            this.NotifyPropertyChanged();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnDelete(object arg)
        {
            if (MessageBox.Show(
                "Delete Resource " + this.Name,
                "Confirmation",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.OnClose(null);
            this.parent.RemoveContent(this);
        }

        protected override object CreateMemento()
        {
            return this.data.Clone(fullCopy: true);
        }

        protected override void RestoreMemento(object memento)
        {
            ResourceEntry source = memento as ResourceEntry;
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
        private void RenameFile(string newName)
        {
            throw new NotImplementedException();
        }

        private void MoveFile(IFolderViewModel newParent)
        {
            throw new NotImplementedException();
        }

        private void OnSelectFile(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
