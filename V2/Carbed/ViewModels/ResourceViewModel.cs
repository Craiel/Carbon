﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;
using Carbed.Views;

using Carbon.Editor.Contracts;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils;

using Microsoft.Win32;

namespace Carbed.ViewModels
{
    public class ResourceViewModel : ContentViewModel, IResourceViewModel
    {
        private readonly ICarbedLogic logic;
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
        private bool forceExport;
        private bool autoUpdateTextures;

        private ColladaInfo colladaSourceInfo;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();
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
                return this.forceExport;
            }

            set
            {
                if (this.forceExport != value)
                {
                    this.forceExport = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool AutoUpdateTextures
        {
            get
            {
                return this.autoUpdateTextures;
            }

            set
            {
                if (this.autoUpdateTextures != value)
                {
                    this.autoUpdateTextures = value;
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
                    this.NotifyPropertyChanged();
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

            this.data.TreeNode = this.parent.Id;
            this.data.Hash = HashUtils.BuildResourceHash(Path.Combine(this.parent.FullPath, this.Name));

            if (this.needReexport || this.forceExport)
            {
                switch (this.data.Type)
                {
                    case ResourceType.Texture:
                    case ResourceType.Raw:
                        {
                            ICarbonResource resource = this.resourceProcessor.ProcessRaw(source);
                            if (resource != null)
                            {
                                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                                this.needReexport = false;
                                this.forceExport = false;
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

                            ICarbonResource resource = this.resourceProcessor.ProcessModel(this.colladaSourceInfo, this.SelectedSourceElement);
                            if (resource != null)
                            {
                                resourceTarget.StoreOrReplace(this.data.Hash, resource);
                                this.needReexport = false;
                                this.forceExport = false;
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
            this.Name = Path.GetFileName(path);
            this.SetTypeByExtension(Path.GetExtension(path));
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
        }

        protected override void OnRefresh(object arg)
        {
            this.textureSynchronizer.Refresh();
            if (this.AutoUpdateTextures)
            {
                this.textureSynchronizer.Synchronize();
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
                this.textureSynchronizer.SetTarget(this.TextureFolder);
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

        private void OnTextureSynchronizerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged("TextureSynchronizer");
        }
    }
}
