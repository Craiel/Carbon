using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Xcd.Scene
{
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class XcdCamera
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("position")]
        public FloatArrayType Position { get; set; }

        [XmlElement("orientation")]
        public FloatArrayType Orientation { get; set; }

        [XmlAttribute("fov")]
        public float FieldOfView { get; set; }

        [XmlElement(ElementName = "Layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "CustomProperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
