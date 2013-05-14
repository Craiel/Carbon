using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdCamera
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlAttribute("position")]
        public FloatArrayAttribute Position { get; set; }

        [XmlAttribute("orientation")]
        public FloatArrayAttribute Orientation { get; set; }

        [XmlAttribute("fov")]
        public float FieldOfView { get; set; }

        [XmlElement(ElementName = "Layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "CustomProperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
