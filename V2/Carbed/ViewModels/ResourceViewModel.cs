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
        private string fileName;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory)
        {
            this.data = data;

            if (this.data.Resource != null && !string.IsNullOrEmpty(this.data.Resource.Path))
            {
                this.fileName = System.IO.Path.GetFileName(this.data.Resource.Path);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.Name ?? "<no name>";
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.fileName))
                {
                    return "<no name>";
                }

                return this.fileName;
            }

            set
            {
                if (this.fileName != value)
                {
                    this.RenameFile(value);
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
                    this.MoveFile(value);
                }
            }
        }

        public bool IsExpanded { get; set; }

        public string FileName
        {
            get
            {
                return this.data.SourcePath;
            }
        }

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
            return this.data.Clone();
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
