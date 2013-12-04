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

        [XmlElement("IncludeSource")]
        public DemonInclude[] SourceIncludes { get; set; }

        [XmlElement("IncludeIntermediate")]
        public DemonInclude[] IntermediateIncludes { get; set; }

        [XmlElement("ColladaExport")]
        public DemonColladaExport[] ColladaExports { get; set; }

        [XmlElement("StageExport")]
        public DemonStageExport[] StageExports { get; set; }

        [XmlElement("Build")]
        public DemonBuildConfig[] Builds { get; set; }
    }
    
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DemonInclude
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [XmlAttribute]
        public string Path { get; set; }
    }

    [Serializable]
    public class DemonColladaExport
    {
        [XmlAttribute]
        public string Pattern { get; set; }

        [XmlAttribute]
        public string Target { get; set; }
    }

    [Serializable]
    public class DemonStageExport
    {
        [XmlAttribute]
        public string Pattern { get; set; }

        [XmlAttribute]
        public string Target { get; set; }
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
