using System;
using System.Windows;

using Carbed.Contracts;

using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public abstract class FolderContentViewModel : DocumentViewModel, IProjectFolderContent
    {
        private readonly SourceFolderContent data;
        
        private IProjectFolderViewModel parent;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected FolderContentViewModel(IEngineFactory factory, SourceFolderContent data)
            : base(factory)
        {
            this.data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.data.Name ?? "<no name>";
            }
        }

        public bool HasName
        {
            get
            {
                return string.IsNullOrEmpty(this.data.Name);
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
                    this.NotifyPropertyChanged("HasName");
                }
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
        
        public bool IsExpanded { get; set; }
        
        public SourceFolderContent Data
        {
            get
            {
                return this.data;
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
            SourceTextureFont source = memento as SourceTextureFont;
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
