using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils;

using Microsoft.Win32;

namespace Carbed.ViewModels
{
    public class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private readonly ResourceEntry data;

        private ICommand commandSelectFile;

        private IFolderViewModel parent;

        private string oldHash;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceViewModel(IEngineFactory factory, ResourceEntry data)
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

        public ResourceType Type
        {
            get
            {
                return this.data.Type;
            }

            set
            {
                if (this.data.Type != value)
                {
                    this.CreateUndoState();
                    this.data.Type = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsValidSource
        {
            get
            {
                string path = this.GetMetaValue(MetaDataKey.SourcePath);
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }

                return System.IO.File.Exists(path);
            }
        }

        public string SourcePath
        {
            get
            {
                string path = this.GetMetaValue(MetaDataKey.SourcePath);
                if (string.IsNullOrEmpty(path))
                {
                    return "<none selected>";
                }

                return path;
            }

            private set
            {
                if (this.GetMetaValue(MetaDataKey.SourcePath) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.SourcePath, value);
                    this.NotifyPropertyChanged();
                }
            }
        }

        public DateTime? LastChangeDate
        {
            get
            {
                string date = this.GetMetaValue(MetaDataKey.LastChangeDate);
                if (string.IsNullOrEmpty(date))
                {
                    return null;
                }

                return Convert.ToDateTime(date);
            }

            private set
            {
                string stringValue = value.ToString();
                if (this.GetMetaValue(MetaDataKey.LastChangeDate) != stringValue)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.LastChangeDate, stringValue);
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

        public void Save(IContentManager target, IResourceManager resourceTarget)
        {
            // Update our parent id information before saving
            if (this.parent == null || this.parent.Id == null)
            {
                throw new InvalidOperationException("Save was called with orphan resource or unsaved parent");
            }

            if (!this.IsNamed)
            {
                throw new DataException("Resource needs to be named before saving");
            }

            if (this.data.TreeNode == null)
            {
                this.data.TreeNode = new ContentLink { Type = ContentLinkType.ResourceTreeNode };
            }

            if (this.data.TreeNode.ContentId != this.parent.Id)
            {
                this.data.TreeNode.ContentId = (int)this.parent.Id;
            }

            // Todo: Duplicate check for resource names
            this.data.Hash = HashUtils.BuildResourceHash(System.IO.Path.Combine(this.parent.FullPath, this.Name));
            this.Save(target);
            
            // Todo: Save the actual Resource now
            if (this.oldHash != null)
            {
                // Todo: resourceTarget.Delete(this.oldHash);
                this.oldHash = null;
            }
            // Todo: Process Resource and store

            this.NotifyPropertyChanged();
        }

        public void Delete(IContentManager target, IResourceManager resourceTarget)
        {
            this.Delete(target);
            
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
            // If we have no hash yet there is nothing to be done with moving
            if (string.IsNullOrEmpty(this.data.Hash))
            {
                this.parent = newParent;
                return;
            }

            this.oldHash = this.data.Hash;
            this.data.Hash = null;
        }

        private void OnSelectFile(object obj)
        {
            var dialog = new OpenFileDialog { CheckFileExists = true, CheckPathExists = true };
            if (dialog.ShowDialog() == true)
            {
                this.SourcePath = dialog.FileName;
            }
        }
    }
}
