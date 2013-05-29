using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    using Core.Editor.Resource.Collada.Data;

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
