using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaSampler2D
    {
        [XmlElement(ElementName = "source")]
        public ColladaSource Source { get; set; }
    }
}
