﻿namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.UtilsWPF;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource;
    using Core.Engine.Resource.Content;

    using GrandSeal.Editor.Contracts;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using Microsoft.Win32;
    
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
        protected ResourceViewModel(IFactory factory)
            : base(factory)
        {
            this.logic = factory.Resolve<IEditorLogic>();
            this.settings = factory.Resolve<IEditorSettings>();
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();
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
                return CarbonFile.FileExists(this.SourceFile);
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
                CarbonDirectory sourcePath = this.SourcePath;
                if (sourcePath == null && value == null)
                {
                    return;
                }

                if (sourcePath == null || !sourcePath.Equals(value))
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
                CarbonFile source = this.SourceFile;
                if (source == null && value == null)
                {
                    return;
                }

                if (source == null || !source.Equals(value))
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

            bool validSource = this.IsValidSource;
            if (this.UsesPath)
            {
                if (!validSource)
                {
                    throw new DataException("Path does not exist for resource " + this.Name);
                }
            }
            else
            {
                if (!validSource)
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

            // See if we have this resource, if not we force anyway
            var existingInfo = resourceTarget.GetInfo(this.data.Hash);
            bool forceSave = this.GetMetaValueBit(MetaDataKey.ResourceCoreFlags, (int)CoreFlags.AlwaysForceSave) ?? existingInfo == null;
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
            if (!CarbonFile.FileExists(file))
            {
                throw new DataException("Invalid file specified for select");
            }

            if (!this.IsNamed)
            {
                this.Name = file.FileName;
            }

            var sourceFile = file.ToRelative<CarbonFile>(this.logic.ProjectLocation);
            this.SourceFile = sourceFile;
            this.SourcePath = new CarbonDirectory(sourceFile.DirectoryName);
            this.SetSettingsByExtension(file.Extension);

            this.CheckSource();
        }

        public virtual void SelectPath(CarbonDirectory path)
        {
            if (!this.UsesPath)
            {
                throw new InvalidOperationException("Viewmodel is not using path");
            }

            this.SourcePath = path.ToRelative<CarbonDirectory>(this.logic.ProjectLocation);

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
            if (!CarbonFile.FileExists(source))
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

        protected override void OnDelete()
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

            this.OnClose();
            this.logic.Delete(this);
        }

        protected override void OnRefresh()
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
                System.Diagnostics.Trace.TraceError("Failed to export raw resource {0}", null, this.SourcePath);
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

        private void OnSelectFile()
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
