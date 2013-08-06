using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Core.Engine.Contracts;
using Core.Engine.Contracts.Resource;
using Core.Engine.Resource;
using Core.Engine.Resource.Content;
using Core.Utils;
using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;

namespace GrandSeal.Editor.ViewModels
{
    using Core.Processing.Contracts;
    using Core.Utils.IO;

    public abstract class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private enum CoreFlags
        {
            AlwaysForceSave = 0,
        }

        private readonly IEditorLogic logic;
        private readonly IEditorSettings settings;
        private readonly IResourceProcessor resourceProcessor;
        private readonly ResourceEntry data;
        
        private ICommand commandSelectFile;
        
        private IFolderViewModel parent;
        
        private long? sourceSize;
        private long? targetSize;

        private string oldHash;

        private bool isUpdatingPreview;
        
        private ImageSource previewImage;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<IEditorLogic>();
            this.settings = factory.Get<IEditorSettings>();
            this.resourceProcessor = factory.Get<IResourceProcessor>();
            this.data = data;
            
            this.NeedSave = data.IsNew;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.Name;
            }
        }

        public override bool IsChanged
        {
            get
            {
                return this.data.IsChanged;
            }
        }

        public int? Id
        {
            get
            {
                return this.data.Id;
            }
        }

        public string Hash
        {
            get
            {
                return this.data.Hash;
            }
        }

        public ResourceType Type
        {
            get
            {
                return this.data.Type;
            }
        }

        public long? SourceSize
        {
            get
            {
                return this.sourceSize;
            }

            set
            {
                if (this.sourceSize != value)
                {
                    this.sourceSize = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public long? TargetSize
        {
            get
            {
                return this.targetSize;
            }

            private set
            {
                if (this.targetSize != value)
                {
                    this.targetSize = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsValidSource
        {
            get
            {
                CarbonFile source = this.SourceFile;
                return !source.IsNull && source.Exists;
            }
        }

        public bool UsesPath { get; protected set; }

        public bool ForceSave
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceCoreFlags, (int)CoreFlags.AlwaysForceSave) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceCoreFlags, (int)CoreFlags.AlwaysForceSave) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceCoreFlags, (int)CoreFlags.AlwaysForceSave, value);
                    this.NotifyPropertyChanged();
                }
            }
        }

        public CarbonDirectory SourcePath
        {
            get
            {
                string path = this.GetMetaValue(MetaDataKey.SourcePath);
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                return new CarbonDirectory(path);
            }

            private set
            {
                if (!this.SourcePath.Equals(value))
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.SourcePath, value.ToString());
                    this.NotifyPropertyChanged();
                }
            }
        }

        public CarbonFile SourceFile
        {
            get
            {
                string path = this.GetMetaValue(MetaDataKey.SourceFile);
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                return new CarbonFile(path);
            }

            private set
            {
                if (!this.SourceFile.Equals(value))
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.SourceFile, value.ToString());
                    this.NotifyPropertyChanged();
                }
            }
        }
        
        public DateTime? LastChangeDate
        {
            get
            {
                long? ticks = this.GetMetaValueLong(MetaDataKey.LastChangeDate);
                if (ticks == null)
                {
                    return null;
                }

                return new DateTime((long)ticks);
            }

            private set
            {
                long? ticks = value == null ? null : (long?)value.Value.Ticks;
                if (this.GetMetaValueLong(MetaDataKey.LastChangeDate) != ticks)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.LastChangeDate, ticks);
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
                // See if we are getting assigned to a different parent node
                if (this.parent != value && this.data.TreeNode != value.Id)
                {
                    this.MoveFile(value);
                }
                else
                {
                    this.parent = value;
                }
            }
        }

        public ImageSource PreviewImage
        {
            get
            {
                if (this.previewImage == null)
                {
                    if (!this.isUpdatingPreview)
                    {
                        this.isUpdatingPreview = true;
                        this.DoUpdatePreview();
                    }

                    return new BitmapImage(StaticResources.NoPreviewImageUri);
                }

                return this.previewImage;
                
            }

            protected set
            {
                this.previewImage = value;
                this.NotifyPropertyChanged();
            }
        }

       public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
        }

        public void Save(IContentManager target, IResourceManager resourceTarget, bool force)
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

            if (this.UsesPath)
            {
                if (this.SourcePath.IsNull || !this.SourcePath.Exists)
                {
                    throw new DataException("Path does not exist for resource " + this.Name);
                }
            }
            else
            {
                if (this.SourceFile.IsNull || !this.SourceFile.Exists)
                {
                    throw new DataException("File does not exist for resource " + this.Name);
                }
            }

            if (this.oldHash != null)
            {
                // Todo: resourceTarget.Delete(this.oldHash);
                this.oldHash = null;
            }

            this.PrepareSave();

            this.data.TreeNode = this.parent.Id;
            this.data.Hash = HashUtils.BuildResourceHash(this.parent.FullPath.ToFile(this.Name).ToString());

            bool forceSave = this.GetMetaValueBit(MetaDataKey.ResourceCoreFlags, (int)CoreFlags.AlwaysForceSave) ?? false;
            if (this.NeedSave || force || forceSave)
            {
                this.DoSave(target, resourceTarget);
            }

            // Get the info of the saved resource to store some meta data about it
            ResourceInfo info = resourceTarget.GetInfo(this.data.Hash);
            if (info != null)
            {
                this.SetMetaValue(MetaDataKey.TargetMd5, HashUtils.Md5ToString(info.Md5));
            }

            // Now save the content and all the meta data as last action
            this.Save(target);

            this.NotifyPropertyChanged();
        }

        public void Delete(IContentManager target, IResourceManager resourceTarget)
        {
            if (!string.IsNullOrEmpty(this.data.Hash) && !string.IsNullOrEmpty(this.GetMetaValue(MetaDataKey.TargetMd5)))
            {
                resourceTarget.Delete(this.data.Hash);
            }
            
            this.Delete(target);

            if (this.parent != null)
            {
                this.parent.RemoveContent(this);
            }

            this.NotifyPropertyChanged();
        }

        public virtual void SelectFile(CarbonFile file)
        {
            if (file.IsNull || !file.Exists)
            {
                throw new DataException("Invalid file specified for select");
            }

            if (!this.IsNamed)
            {
                this.Name = file.FileName;
            }

            this.SetSettingsByExtension(file.Extension);

            this.CheckSource();
        }

        public virtual void SelectPath(CarbonDirectory path)
        {
            if (!this.UsesPath)
            {
                throw new InvalidOperationException("Viewmodel is not using path");
            }

            this.SourcePath = path;

            // Todo:
            // Set settings by path content

            this.CheckSource();
        }

        public void CheckSource()
        {
            if (this.UsesPath)
            {
                this.CheckSourcePath();
            }
            else
            {
                this.CheckSourceFile();
            }
        }

        public void CheckSourcePath()
        {
            System.Diagnostics.Trace.TraceWarning("CheckSourcePath is not implemented");
        }

        public void CheckSourceFile()
        {
            CarbonFile source = this.SourceFile;
            if (source.IsNull || !source.Exists)
            {
                this.sourceSize = null;
                this.NeedSave = true;
                return;
            }
            
            DateTime changeTime = source.LastWriteTime;
            if (this.LastChangeDate != changeTime)
            {
                this.NeedSave = true;
                this.LastChangeDate = changeTime;
            }

            if (string.IsNullOrEmpty(this.GetMetaValue(MetaDataKey.TargetMd5)))
            {
                this.NeedSave = true;
            }

            this.SourceSize = source.Size;
        }

        public override void Load()
        {
            base.Load();

            this.CheckSource();
        }

        public void UpdateAutoCompletion(IList<ICompletionData> completionList, string context = null)
        {
            // Todo
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool NeedSave { get; set; }

        protected ResourceEntry Data
        {
            get
            {
                return this.data;
            }
        }

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
            this.logic.Delete(this);
        }

        protected override void OnRefresh(object arg)
        {
            this.CheckSource();
        }

        protected override object CreateMemento()
        {
            return this.data.Clone(fullCopy: true);
        }

        protected override void RestoreMemento(object memento)
        {
            var source = memento as ResourceEntry;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.CreateUndoState();
            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }

        protected virtual void PrepareSave()
        {
        }

        protected virtual void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            ICarbonResource resource;
            if (this.UsesPath)
            {
                resource = this.resourceProcessor.ProcessRaw(this.SourcePath);
            }
            else
            {
                resource = this.resourceProcessor.ProcessRaw(this.SourceFile);
            }

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                this.Log.Error("Failed to export raw resource {0}", null, this.SourcePath);
            }
        }

        protected virtual void SetSettingsByExtension(string extension)
        {
        }

        protected virtual ImageSource GetPreviewImage()
        {
            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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
            this.NeedSave = true;
        }

        private void OnSelectFile(object obj)
        {
            var dialog = new OpenFileDialog { CheckFileExists = true, CheckPathExists = true };
            if (dialog.ShowDialog() == true)
            {
                this.SelectFile(new CarbonFile(dialog.FileName));
            }
        }

        private void DoUpdatePreview()
        {
            ImageSource source = this.GetPreviewImage();
            if (source != null)
            {
                this.PreviewImage = source;
            }

            this.isUpdatingPreview = false;
        }
    }
}
