namespace GrandSeal.DataDemon.Logic
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    public class DemonArguments
    {
        public string Config { get; set; }
    }

    [Serializable]
    [XmlRoot]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DemonConfig
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonConfig()
        {
            this.RefreshInterval = 5000; // 5 seconds In milliseconds
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [XmlAttribute]
        public int RefreshInterval { get; set; }

        [XmlElement("DataConversion")]
        public DemonConversionConfig[] DataConversions { get; set; }

        [XmlElement("Build")]
        public DemonBuildConfig[] Builds { get; set; }
    }

    [Serializable]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DemonConversionConfig
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonConversionConfig()
        {
            this.AutoRefresh = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string SourcePath { get; set; }

        [XmlAttribute]
        public string TargetPath { get; set; }

        [XmlAttribute]
        public bool AutoRefresh { get; set; }
    }

    [Serializable]
    public class DemonBuildConfig
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ProjectRoot { get; set; }
    }
}
