using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectParameter
    {
        [XmlAttribute(AttributeName = "sid")]
        public string Sid { get; set; }

        [XmlElement(ElementName = "sampler2D")]
        public ColladaSampler2D Sampler2D { get; set; }

        [XmlElement(ElementName = "surface")]
        public ColladaSurface Surface { get; set; }
    }
}
