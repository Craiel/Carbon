﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class ResourceModelViewModel : ResourceViewModel, IResourceModelViewModel
    {
        private enum Flags
        {
            AutoUpdateTextures = 0
        }

        private readonly ICarbedLogic logic;
        private readonly ICarbedSettings settings;
        private readonly IResourceProcessor resourceProcessor;
        private readonly List<string> sourceElements;

        private IFolderViewModel textureFolder;
        private ITextureSynchronizer textureSynchronizer;

        private ICommand commandSelectTextureFolder;

        private ColladaInfo colladaSourceInfo;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceModelViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.settings = factory.Get<ICarbedSettings>();
            this.resourceProcessor = factory.Get<IResourceProcessor>();

            this.sourceElements = new List<string>();
            this.textureSynchronizer = factory.Get<ITextureSynchronizer>();
            this.textureSynchronizer.PropertyChanged += this.OnTextureSynchronizerChanged;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool AutoUpdateTextures
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.AutoUpdateTextures) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.AutoUpdateTextures) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)Flags.AutoUpdateTextures, value);
                    this.NeedReExport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsHavingSourceElements
        {
            get
            {
                return this.sourceElements.Count > 0;
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
                    this.NeedReExport = true;
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

        public ICommand CommandSelectTextureFolder
        {
            get
            {
                return this.commandSelectTextureFolder ?? (this.commandSelectTextureFolder = new RelayCommand(this.OnSelectTextureFolder));
            }
        }

        public override void Load()
        {
            base.Load();

            this.UpdateSourceElements();

            this.textureFolder = this.logic.LocateFolder(this.GetMetaValue(MetaDataKey.TextureFolder));
            this.textureSynchronizer.SetTarget(this.textureFolder);
            this.textureSynchronizer.Refresh();
        }

        public override void SelectFile(string path)
        {
            base.SelectFile(path);

            this.UpdateSourceElements();

            if (this.settings.ModelTextureAutoCreateFolder && this.settings.ModelTextureParentFolder != null && this.TextureFolder == null)
            {
                var textureParent = this.logic.LocateFolder(this.settings.ModelTextureParentFolder);
                if (textureParent != null)
                {
                    var folder = textureParent.AddFolder();
                    folder.Name = string.Concat(this.Name, "_textures");
                    this.AutoUpdateTextures = true;
                    this.TextureFolder = folder;
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnDelete(object arg)
        {
            if (MessageBox.Show(
                "Delete Model " + this.Name,
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
            base.OnRefresh(arg);

            this.textureSynchronizer.Refresh();
            if (this.AutoUpdateTextures)
            {
                this.textureSynchronizer.Synchronize();
            }
        }

        protected override void PrepareSave()
        {
            if (this.textureFolder != null)
            {
                this.SetMetaValue(MetaDataKey.TextureFolder, this.textureFolder.Hash);
            }
        }

        protected override void DoExport(IContentManager target, IResourceManager resourceTarget)
        {
            if (this.colladaSourceInfo == null)
            {
                this.UpdateSourceElements();
            }

            string texturePath = this.textureFolder == null ? null : this.textureFolder.FullPath;
            ICarbonResource resource = this.resourceProcessor.ProcessModel(this.colladaSourceInfo, this.SelectedSourceElement, texturePath);
            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedReExport = false;
            }
            else
            {
                this.Log.Error("Failed to export Model resource {0}, with target {1}", null, this.SourcePath, this.SelectedSourceElement);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectTextureFolder(object obj)
        {
            var dialog = new SelectFolderDialog(this.logic);
            if (dialog.ShowDialog() == true)
            {
                this.TextureFolder = dialog.SelectedFolder;
            }
        }

        private void OnTextureSynchronizerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged("TextureSynchronizer");
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
                this.Log.Error("Could not get collada info of source file for mesh, please check the format", e);
            }

            if (string.IsNullOrEmpty(selection) || this.sourceElements.Contains(selection))
            {
                return;
            }

            this.SelectedSourceElement = null;
        }
    }
}