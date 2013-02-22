using System;
using System.Windows;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public abstract class ResourceViewModel : DocumentViewModel, IResourceViewModel
    {
        private readonly ResourceEntry data;

        private ICommand commandSelectFile;

        private IFolderViewModel parent;

        private MetaDataEntry name;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory)
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

        public override string Name
        {
            get
            {
                if (this.name == null || string.IsNullOrEmpty(this.name.Value))
                {
                    return "<no name>";
                }

                return this.name.Value;
            }

            set
            {
                /*if (this.fileName != value)
                {
                    this.RenameFile(value);
                }*/
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

        public bool IsExpanded { get; set; }

        public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
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
            this.parent.DeleteContent(this);
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
