using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Carbed.Contracts;
using Carbed.Logic.MVVM;
using Carbed.Views;

using Carbon.Editor.Contracts;
using Carbon.Editor.Processors;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

using Microsoft.Win32;

namespace Carbed.ViewModels
{
    internal enum ResourceToolFlags
    {
        AlwaysForceExport = 0,
        AutoUpdateTextures = 1,

        CompressTexture = 2,
        IsNormalMap = 3,
        ConvertToNormalMap = 4,
    }

    public class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly ICarbedSettings settings;
        private readonly IResourceProcessor resourceProcessor;
        private readonly ResourceEntry data;
        private readonly List<string> sourceElements;

        private ICommand commandSelectFile;
        private ICommand commandSelectTextureFolder;

        private IFolderViewModel parent;
        private IFolderViewModel textureFolder;
        private ITextureSynchronizer textureSynchronizer;

        private long? sourceSize;
        private long? targetSize;

        private string oldHash;

        private bool needReexport;
        private bool isUpdatingPreview;
        private bool scriptWasChanged;
        private bool keepLocalScriptChanges;
        
        private ColladaInfo colladaSourceInfo;

        private ImageSource previewImage;

        private ITextSource scriptDocument;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.settings = factory.Get<ICarbedSettings>();
            this.resourceProcessor = factory.Get<IResourceProcessor>();
            this.data = data;
            this.sourceElements = new List<string>();
            this.textureSynchronizer = factory.Get<ITextureSynchronizer>();
            this.textureSynchronizer.PropertyChanged += this.OnTextureSynchronizerChanged;

            this.needReexport = data.IsNew;
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

            set
            {
                if (this.data.Type != value)
                {
                    this.CreateUndoState();
                    this.data.Type = value;
                    this.UpdateTypeStatus();
                    this.NotifyPropertyChanged();

                    if (value == ResourceType.Model)
                    {
                        this.UpdateSourceElements();
                    }
                }
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

        public bool CanChangeType
        {
            get
            {
                return this.data.IsNew;
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

        public bool IsHavingSourceElements
        {
            get
            {
                return this.sourceElements.Count > 0;
            }
        }

        public bool ForceExport
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AlwaysForceExport) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AlwaysForceExport) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AlwaysForceExport, value);
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool AutoUpdateTextures
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AutoUpdateTextures) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AutoUpdateTextures) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AutoUpdateTextures, value);
                    this.needReexport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool CompressTexture
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.CompressTexture) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.CompressTexture) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.CompressTexture, value);
                    this.needReexport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsNormalMap
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.IsNormalMap) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.IsNormalMap) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.IsNormalMap, value);
                    this.needReexport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool ConvertToNormalMap
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.ConvertToNormalMap) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.ConvertToNormalMap) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.ConvertToNormalMap, value);
                    this.needReexport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public TextureTargetFormat TextureTargetFormat
        {
            get
            {
                long value = this.GetMetaValueInt(MetaDataKey.TextureTargetFormat) ?? 0;
                return (TextureTargetFormat)value;
            }

            set
            {
                if (this.GetMetaValueInt(MetaDataKey.TextureTargetFormat) != (int)value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.TextureTargetFormat, (int)value);
                    this.needReexport = true;
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

        public ReadOnlyCollection<string> SourceElements
        {
            get
            {
                return this.sourceElements.AsReadOnly();
            }
        }

        public string SelectedSourceElement
        {
            get
            {
                return this.GetMetaValue(MetaDataKey.SourceElement);
            }

            set
            {
                if (this.GetMetaValue(MetaDataKey.SourceElement) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.SourceElement, value);
                    this.needReexport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ITextureSynchronizer TextureSynchronizer
        {
            get
            {
                return this.textureSynchronizer;
            }

            private set
            {
                if (this.textureSynchronizer != value)
                {
                    this.textureSynchronizer = value;
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

        public IFolderViewModel TextureFolder
        {
            get
            {
                return this.textureFolder;
            }

            set
            {
                if (this.textureFolder != value)
                {
                    this.textureFolder = value;
                    this.textureSynchronizer.SetTarget(value);
                    this.NotifyPropertyChanged();
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
                        new Task(this.UpdatePreviewImage).Start();
                    }

                    return new BitmapImage(StaticResources.NoPreviewImageUri);
                }

                return this.previewImage;
                
            }

            private set
            {
                this.previewImage = value;
                this.NotifyPropertyChanged();
            }
        }

        public ITextSource ScriptDocument
        {
            get
            {
                if (this.scriptDocument == null)
                {
                    this.scriptDocument = this.GetScriptingSource();
                    this.scriptDocument.TextChanged += this.OnScriptDocumentChanged;
                }

                return this.scriptDocument;
            }
        }

        public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
        }

        public ICommand CommandSelectTextureFolder
        {
            get
            {
                return this.commandSelectTextureFolder ?? (this.commandSelectTextureFolder = new RelayCommand(this.OnSelectTextureFolder));
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

            string source = this.SourcePath;
            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                throw new DataException("File does not exist for resource " + this.Name);
            }
            
            if (this.oldHash != null)
            {
                // Todo: resourceTarget.Delete(this.oldHash);
                this.oldHash = null;
            }

            if (this.textureFolder != null)
            {
                this.SetMetaValue(MetaDataKey.TextureFolder, this.textureFolder.Hash);
            }

            // See if we have changes in the script that need to be saved before export
            if (this.scriptWasChanged && this.scriptDocument != null)
            {
                Application.Current.Dispatcher.Invoke(this.SaveScript);
            }

            this.data.TreeNode = this.parent.Id;
            this.data.Hash = HashUtils.BuildResourceHash(Path.Combine(this.parent.FullPath, this.Name));

            bool force = this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)ResourceToolFlags.AlwaysForceExport) ?? false;
            if (this.needReexport || force)
            {
                switch (this.data.Type)
                {
                    case ResourceType.Texture:
                        {
                            ICarbonResource resource;
                            if (!this.CompressTexture)
                            {
                                resource = this.resourceProcessor.ProcessRaw(source);
                            }
                            else
                            {
                                var options = new TextureProcessingOptions
                                    {
                                        Format = this.TextureTargetFormat,
                                        ConvertToNormalMap = this.ConvertToNormalMap,
                                        IsNormalMap = this.IsNormalMap
                                    };
                                resource = this.resourceProcessor.ProcessTexture(source, options);
                            }
                            
                            if (resource != null)
                            {
                                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                                this.needReexport = false;
                            }
                            else
                            {
                                this.Log.Error("Failed to export raw resource {0}", null, this.SourcePath);
                            }

                            break;
                        }

                    case ResourceType.Script:
                    case ResourceType.Raw:
                        {
                            ICarbonResource resource = this.resourceProcessor.ProcessRaw(source);
                            if (resource != null)
                            {
                                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                                this.needReexport = false;
                            }
                            else
                            {
                                this.Log.Error("Failed to export raw resource {0}", null, this.SourcePath);
                            }

                            break;
                        }

                    case ResourceType.Model:
                        {
                            if (this.colladaSourceInfo == null)
                            {
                                this.UpdateSourceElements();
                            }

                            string texturePath = this.textureFolder == null ? null : this.textureFolder.FullPath;
                            ICarbonResource resource = this.resourceProcessor.ProcessModel(this.colladaSourceInfo, this.SelectedSourceElement, texturePath);
                            if (resource != null)
                            {
                                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                                this.needReexport = false;
                            }
                            else
                            {
                                this.Log.Error("Failed to export Model resource {0}, with target {1}", null, this.SourcePath, this.SelectedSourceElement);
                            }

                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException("Export is not implemented for " + this.data.Type);
                        }
                }
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

        public void SelectFile(string path)
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

            if (this.CanChangeType)
            {
                this.SetTypeByExtension(Path.GetExtension(path));
            }

            this.CheckSource();
        }

        public void CheckSource()
        {
            string path = this.SourcePath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                this.sourceSize = null;
                this.needReexport = true;
                return;
            }
            
            DateTime changeTime = File.GetLastWriteTime(path);
            if (this.LastChangeDate != changeTime)
            {
                this.needReexport = true;
                this.LastChangeDate = changeTime;
                if (this.Type == ResourceType.Script && !keepLocalScriptChanges)
                {
                    this.HandleSourceScriptChange();
                }
                this.UpdateSourceElements();
            }

            if (string.IsNullOrEmpty(this.GetMetaValue(MetaDataKey.TargetMd5)))
            {
                this.needReexport = true;
            }

            var info = new FileInfo(path);
            this.SourceSize = info.Length;
        }

        public override void Load()
        {
            base.Load();

            this.CheckSource();

            if (this.Type == ResourceType.Model)
            {
                this.UpdateSourceElements();

                this.textureFolder = this.logic.LocateFolder(this.GetMetaValue(MetaDataKey.TextureFolder));
                this.textureSynchronizer.SetTarget(this.textureFolder);
                this.textureSynchronizer.Refresh();
            }
        }

        public void UpdateAutoCompletion(IList<ICompletionData> completionList, string context = null)
        {
            // Todo
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
            this.logic.Delete(this);

            if (this.Type == ResourceType.Model)
            {
                if (this.AutoUpdateTextures && this.TextureFolder != null)
                {
                    this.logic.Delete(this.TextureFolder);
                }
            }
        }

        protected override void OnRefresh(object arg)
        {
            this.CheckSource();

            if (this.Type == ResourceType.Model)
            {
                this.textureSynchronizer.Refresh();
                if (this.AutoUpdateTextures)
                {
                    this.textureSynchronizer.Synchronize();
                }
            }
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
            this.needReexport = true;
        }

        private void OnSelectFile(object obj)
        {
            var dialog = new OpenFileDialog { CheckFileExists = true, CheckPathExists = true };
            if (dialog.ShowDialog() == true)
            {
                this.SelectFile(dialog.FileName);
            }
        }

        private void OnSelectTextureFolder(object obj)
        {
            var dialog = new SelectFolderDialog(this.logic);
            if (dialog.ShowDialog() == true)
            {
                this.TextureFolder = dialog.SelectedFolder;
            }
        }

        private void SetTypeByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                this.Type = ResourceType.Unknown;
                return;
            }

            switch (extension.ToLower())
            {
                case ".dds":
                    {
                        this.Type = ResourceType.Texture;
                        this.CompressTexture = false;
                        break;
                    }

                case ".png":
                case ".tif":
                case ".jpg":
                case ".tga":
                    {
                        this.Type = ResourceType.Texture;
                        this.CompressTexture = true;
                        this.TextureTargetFormat = TextureTargetFormat.DDSDxt1;
                        break;
                    }

                case ".lua":
                    {
                        this.Type = ResourceType.Script;
                        break;
                    }

                case ".dae":
                    {
                        this.Type = ResourceType.Model;
                        break;
                    }

                default:
                    {
                        this.Type = ResourceType.Unknown;
                        break;
                    }
            }
        }

        private void UpdateTypeStatus()
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
        }

        private void OnTextureSynchronizerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged("TextureSynchronizer");
        }

        private void UpdatePreviewImage()
        {
            switch (Path.GetExtension(this.SourcePath))
            {
                case ".png":
                case ".tif":
                case ".jpg":
                    {
                        this.PreviewImage = WPFUtilities.FileToImage(this.SourcePath);
                        break;
                    }
            }

            this.isUpdatingPreview = false;
        }

        private void UpdateSourceElements()
        {
            string path = this.SourcePath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            string selection = this.SelectedSourceElement;
            this.sourceElements.Clear();
            switch (this.Type)
            {
                case ResourceType.Model:
                    {
                        try
                        {
                            this.colladaSourceInfo = new ColladaInfo(path);
                            this.textureSynchronizer.SetSource(this.colladaSourceInfo);
                            foreach (ColladaMeshInfo meshInfo in this.colladaSourceInfo.MeshInfos)
                            {
                                this.sourceElements.Add(meshInfo.Name);
                            }
                        }
                        catch (Exception e)
                        {
                            this.Log.Error("Could not get collada info of source file for mesh, please check the format");
                        }
                        break;
                    }
            }

            if (string.IsNullOrEmpty(selection) || this.sourceElements.Contains(selection))
            {
                return;
            }

            this.SelectedSourceElement = null;
        }

        private void OnScriptDocumentChanged(object sender, EventArgs e)
        {
            this.scriptWasChanged = true;
        }

        private ITextSource GetScriptingSource()
        {
            if (!File.Exists(this.SourcePath))
            {
                return new TextDocument();
            }

            using (var reader = new StreamReader(this.SourcePath))
            {
                return new TextDocument(reader.ReadToEnd());
            }
        }

        private void UnloadScript()
        {
            if (this.scriptDocument != null)
            {
                this.scriptDocument.TextChanged -= this.OnScriptDocumentChanged;
                this.scriptDocument = null;
                this.NotifyPropertyChanged("ScriptDocument");
            }
        }

        private void HandleSourceScriptChange()
        {
            if (this.scriptWasChanged)
            {
                if (
                    MessageBox.Show(
                        "Source script was changed and you have un-saved modifications. Do you want to reload the source?\n(This will discard all local changes)",
                        "Source changed",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    this.keepLocalScriptChanges = true;
                    return;
                }
            }

            this.UnloadScript();
        }

        private void SaveScript()
        {
            using (var writer = new StreamWriter(this.SourcePath, false))
            {
                writer.Write(this.scriptDocument.Text);
            }

            this.scriptWasChanged = false;
            this.keepLocalScriptChanges = false;
        }

    }
}
