using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdMesh
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlElement(ElementName = "Transform")]
        public XcdTransform Transform { get; set; }

        [XmlElement(ElementName = "Layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "CustomProperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
