namespace GrandSeal.Editor.Logic
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IO;

    using GrandSeal.Editor.Contracts;

    [Serializable]
    public class EditorSettingsData
    {
        [XmlElement]
        public string TextureToolsFolder { get; set; }

        [XmlElement]
        public string ModelTextureParentFolderHash { get; set; }

        [XmlElement]
        public bool ModelTextureAutoCreateFolder { get; set; }
    }

    public class EditorSettings : BaseViewModel, IEditorSettings
    {
        private static readonly XmlSerializer serializer;

        private const string SettingsFileName = "settings.xml";

        private readonly IResourceProcessor resourceProcessor;

        private EditorSettingsData data;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EditorSettings()
        {
            serializer = new XmlSerializer(typeof(EditorSettingsData));
        }

        public EditorSettings(IFactory factory)
        {
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();

            this.Reset();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory TextureToolsFolder
        {
            get
            {
                return new CarbonDirectory(this.data.TextureToolsFolder);
            }

            set
            {
                if (this.data.TextureToolsFolder != value.ToString())
                {
                    this.data.TextureToolsFolder = value.ToString();
                    this.resourceProcessor.TextureToolsPath = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string ModelTextureParentFolderHash
        {
            get
            {
                return this.data.ModelTextureParentFolderHash;
            }

            set
            {
                if (this.data.ModelTextureParentFolderHash != value)
                {
                    this.data.ModelTextureParentFolderHash = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool ModelTextureAutoCreateFolder
        {
            get
            {
                return this.data.ModelTextureAutoCreateFolder;
            }

            set
            {
                if (this.data.ModelTextureAutoCreateFolder != value)
                {
                    this.data.ModelTextureAutoCreateFolder = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public void Save(CarbonDirectory projectFolder)
        {
            CarbonFile file = projectFolder.ToFile(SettingsFileName);
            using (XmlWriter writer = file.OpenXmlWrite())
            {
                serializer.Serialize(writer, this.data);
            }
        }

        public void Load(CarbonDirectory projectFolder)
        {
            CarbonFile file = projectFolder.ToFile(SettingsFileName);
            if (!file.Exists)
            {
                this.Reset();
                return;
            }

            using (XmlReader reader = file.OpenXmlRead())
            {
                this.data = serializer.Deserialize(reader) as EditorSettingsData;
                if (this.data == null)
                {
                    throw new InvalidOperationException("Settings data was null, this is not a valid state");
                }

                // Update the dependencies directly that we know of
                this.resourceProcessor.TextureToolsPath = this.TextureToolsFolder;

                // Notify
                this.NotifyPropertyChanged();
            }
        }

        public void Reset()
        {
            this.data = new EditorSettingsData();
            this.TextureToolsFolder = new CarbonDirectory("TexTools");
            this.ModelTextureParentFolderHash = null;
            this.ModelTextureAutoCreateFolder = false;
        }
    }
}
