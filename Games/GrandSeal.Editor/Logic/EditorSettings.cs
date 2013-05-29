using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using GrandSeal.Editor.Contracts;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.Logic
{
    using Core.Editor.Contracts;

    [Serializable]
    public class EditorSettingsData
    {
        [XmlElement]
        public string TextureToolsFolder { get; set; }

        [XmlElement]
        public string ModelTextureParentFolder { get; set; }

        [XmlElement]
        public bool ModelTextureAutoCreateFolder { get; set; }
    }

    public class EditorSettings : EditorBase, IEditorSettings
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

        public EditorSettings(IEngineFactory factory)
        {
            this.resourceProcessor = factory.Get<IResourceProcessor>();

            this.Reset();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string TextureToolsFolder
        {
            get
            {
                return this.data.TextureToolsFolder;
            }

            set
            {
                if (this.data.TextureToolsFolder != value)
                {
                    this.data.TextureToolsFolder = value;
                    this.resourceProcessor.TextureToolsPath = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string ModelTextureParentFolder
        {
            get
            {
                return this.data.ModelTextureParentFolder;
            }

            set
            {
                if (this.data.ModelTextureParentFolder != value)
                {
                    this.data.ModelTextureParentFolder = value;
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

        public void Save(string projectFolder)
        {
            string file = Path.Combine(projectFolder ?? string.Empty, SettingsFileName);
            using (XmlWriter writer = new XmlTextWriter(file, null))
            {
                serializer.Serialize(writer, this.data);
            }
        }

        public void Load(string projectFolder)
        {
            string file = Path.Combine(projectFolder ?? string.Empty, SettingsFileName);
            if (!File.Exists(file))
            {
                this.Reset();
                return;
            }

            using (XmlReader reader = new XmlTextReader(file))
            {
                this.data = serializer.Deserialize(reader) as EditorSettingsData;
                if (this.data == null)
                {
                    throw new InvalidOperationException("Settings data was null, this is not a valid state");
                }

                // Update the dependencies directly that we know of
                this.resourceProcessor.TextureToolsPath = this.data.TextureToolsFolder;

                // Notify
                this.NotifyPropertyChanged();
            }
        }

        public void Reset()
        {
            this.data = new EditorSettingsData();
            this.TextureToolsFolder = "TexTools";
            this.ModelTextureParentFolder = null;
            this.ModelTextureAutoCreateFolder = false;
        }
    }
}
