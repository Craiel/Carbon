namespace GrandSeal.Editor.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Input;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;
    using CarbonCore.UtilsWPF;

    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Logic.MVVM;
    using GrandSeal.Editor.Views;

    using Microsoft.Win32;

    public class EditorSettingsViewModel : DocumentViewModel, IEditorSettingsViewModel
    {
        private readonly IEditorSettings settings;
        private readonly IEditorLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EditorSettingsViewModel(IFactory factory)
            : base(factory)
        {
            this.settings = factory.Resolve<IEditorSettings>();
            this.logic = factory.Resolve<IEditorLogic>();
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

        public CarbonDirectory TextureToolsFolder
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
                return this.logic.LocateFolder(this.settings.ModelTextureParentFolderHash);
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
            this.NotifyPropertyChangedExplicit(e.PropertyName);
        }

        private void OnSelectModelTextureParentFolder()
        {
            var dialog = new SelectFolderDialog(this.logic);
            if (dialog.ShowDialog() == true)
            {
                this.settings.ModelTextureParentFolderHash = dialog.SelectedFolder.Hash;
            }
        }

        private void OnSelectTextureToolsFolder()
        {
            var dialog = new OpenFileDialog { CheckFileExists = false, CheckPathExists = true };
            if (dialog.ShowDialog() == true)
            {
                this.TextureToolsFolder = new CarbonFile(dialog.FileName).GetDirectory();
            }
        }
        
        private void OnCommandReset()
        {
            this.settings.Reset();
        }
    }
}
