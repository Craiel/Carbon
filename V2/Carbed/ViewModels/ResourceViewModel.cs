using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Editor.Contracts;
using Carbon.Editor.Processors;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils;

using ICSharpCode.AvalonEdit.CodeCompletion;

using Microsoft.Win32;

namespace Carbed.ViewModels
{
    public abstract class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private enum CoreFlags
        {
            AlwaysForceSave = 0,
        }

        private readonly ICarbedLogic logic;
        private readonly ICarbedSettings settings;
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
            this.logic = factory.Get<ICarbedLogic>();
            this.settings = factory.Get<ICarbedSettings>();
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
                string path = this.GetMetaValue(MetaDataKey.SourcePath);
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }

                return File.Exists(path);
            }
        }

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

            if (string.IsNullOrEmpty(this.SourcePath) || !File.Exists(this.SourcePath))
            {
                throw new DataException("File does not exist for resource " + this.Name);
            }
            
            if (this.oldHash != null)
            {
                // Todo: resourceTarget.Delete(this.oldHash);
                this.oldHash = null;
            }

            this.PrepareSave();

            this.data.TreeNode = this.parent.Id;
            this.data.Hash = HashUtils.BuildResourceHash(Path.Combine(this.parent.FullPath, this.Name));

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

        public virtual void SelectFile(string path)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new DataException("Invalid file specified for select");
            }

            this.SourcePath = path;
            if (!this.IsNamed)
            {
                this.Name = Path.GetFileName(path);
            }

            this.SetSettingsByExtension(Path.GetExtension(path));

            this.CheckSource();
        }

        public void CheckSource()
        {
            string path = this.SourcePath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                this.sourceSize = null;
                this.NeedSave = true;
                return;
            }
            
            DateTime changeTime = File.GetLastWriteTime(path);
            if (this.LastChangeDate != changeTime)
            {
                this.NeedSave = true;
                this.LastChangeDate = changeTime;
            }

            if (string.IsNullOrEmpty(this.GetMetaValue(MetaDataKey.TargetMd5)))
            {
                this.NeedSave = true;
            }

            var info = new FileInfo(path);
            this.SourceSize = info.Length;
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
            ICarbonResource resource = this.resourceProcessor.ProcessRaw(this.SourcePath);
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
                this.SelectFile(dialog.FileName);
            }
        }

        /*private void UpdateTypeStatus()
        {
            // Clear out some type specific editor data that will be re-done dynamically if needed
            this.UnloadScript();
            this.previewImage = null;

            switch (this.data.Type)
            {
                case ResourceType.Model:
                    {
                        if (this.settings.ModelTextureAutoCreateFolder && this.settings.ModelTextureParentFolder != null && this.TextureFolder == null)
                        {
                            var textureParent = this.logic.LocateFolder(this.settings.ModelTextureParentFolder);
                            if (textureParent == null)
                            {
                                break;
                            }

                            var folder = textureParent.AddFolder();
                            folder.Name = string.Concat(this.Name, "_textures");
                            this.AutoUpdateTextures = true;
                            this.TextureFolder = folder;
                        }

                        break;
                    }

                case ResourceType.Script:
                    {
                        this.ForceExport = true;
                        break;
                    }
            }
        }*/

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
