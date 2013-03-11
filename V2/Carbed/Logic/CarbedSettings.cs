﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Carbed.Contracts;

using Carbon.Editor.Contracts;
using Carbon.Engine.Contracts;

namespace Carbed.Logic
{
    [Serializable]
    public class CarbedSettingsData
    {
        [XmlElement]
        public string TextureToolsFolder { get; set; }
    }

    public class CarbedSettings : CarbedBase, ICarbedSettings
    {
        private static readonly XmlSerializer serializer;

        private const string SettingsFileName = "settings.xml";

        private readonly IResourceProcessor resourceProcessor;

        private CarbedSettingsData data;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static CarbedSettings()
        {
            serializer = new XmlSerializer(typeof(CarbedSettingsData));
            SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Carbed");
        }

        public CarbedSettings(IEngineFactory factory)
        {
            this.resourceProcessor = factory.Get<IResourceProcessor>();

            this.Reset();
            this.Load();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static string SettingsFolder;

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

        public void Save()
        {
            string file = Path.Combine(SettingsFolder, SettingsFileName);
            if (!File.Exists(file))
            {
                return;
            }

            using (XmlWriter writer = new XmlTextWriter(file, null))
            {
                serializer.Serialize(writer, this.data);
            }
        }

        public void Load()
        {
            string file = Path.Combine(SettingsFolder, SettingsFileName);
            if (!File.Exists(file))
            {
                return;
            }

            using (XmlReader reader = new XmlTextReader(file))
            {
                this.data = serializer.Deserialize(reader) as CarbedSettingsData;
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
            this.data = new CarbedSettingsData();
            this.TextureToolsFolder = "TexTools";
        }
    }
}