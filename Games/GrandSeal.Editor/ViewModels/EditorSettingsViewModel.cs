﻿using System.ComponentModel;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;
using GrandSeal.Editor.Views;

using Core.Engine.Contracts;

using Microsoft.Win32;

namespace GrandSeal.Editor.ViewModels
{
    public class EditorSettingsViewModel : DocumentViewModel, IEditorSettingsViewModel
    {
        private readonly IEditorSettings settings;
        private readonly IEditorLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EditorSettingsViewModel(IEngineFactory factory)
            : base(factory)
        {
            this.settings = factory.Get<IEditorSettings>();
            this.logic = factory.Get<IEditorLogic>();
            this.settings.PropertyChanged += this.OnSettingsChanged;

            this.CommandSelectTextureToolsFolder = new RelayCommand(this.OnSelectTextureToolsFolder);
            this.CommandSelectModelTextureParentFolder = new RelayCommand(this.OnSelectModelTextureParentFolder);

            this.CommandReset = new RelayCommand(this.OnCommandReset);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return "GrandSeal.Editor Settings";
            }
        }

        public string TextureToolsFolder
        {
            get
            {
                return this.settings.TextureToolsFolder;
            }

            set
            {
                this.settings.TextureToolsFolder = value;
            }
        }

        public IFolderViewModel ModelTextureParentFolder
        {
            get
            {
                return this.logic.LocateFolder(this.settings.ModelTextureParentFolder);
            }
        }

        public bool ModelTextureAutoCreateFolder
        {
            get
            {
                return this.settings.ModelTextureAutoCreateFolder;
            }

            set
            {
                this.settings.ModelTextureAutoCreateFolder = value;
            }
        }

        public ICommand CommandSelectTextureToolsFolder { get; private set; }
        public ICommand CommandSelectModelTextureParentFolder { get; private set; }

        public ICommand CommandReset { get; private set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public override string Name { get; set; }
        
        protected override object CreateMemento()
        {
            return null;
        }

        protected override void RestoreMemento(object memento)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(e.PropertyName);
        }

        private void OnSelectModelTextureParentFolder(object obj)
        {
            var dialog = new SelectFolderDialog(this.logic);
            if (dialog.ShowDialog() == true)
            {
                this.settings.ModelTextureParentFolder = dialog.SelectedFolder.Hash;
            }
        }

        private void OnSelectTextureToolsFolder(object obj)
        {
            var dialog = new OpenFileDialog() { CheckFileExists = false, CheckPathExists = true };
            if (dialog.ShowDialog() == true)
            {
                this.TextureToolsFolder = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }
        
        private void OnCommandReset(object obj)
        {
            this.settings.Reset();
        }
    }
}