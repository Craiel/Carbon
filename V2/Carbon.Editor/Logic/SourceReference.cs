using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Logic
{
    [Serializable]
    public enum SourceReferenceType
    {
        File,
        Internal
    }

    [Serializable]
    public class SourceReference
    {
        [XmlAttribute]
        public SourceReferenceType Type { get; set; }

        [XmlAttribute]
        public string Reference { get; set; }
    }
}
