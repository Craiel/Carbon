using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdProperty
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }
    }
}
