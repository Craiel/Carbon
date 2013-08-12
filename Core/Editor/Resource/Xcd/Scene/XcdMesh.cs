namespace Core.Processing.Resource.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class XcdMesh
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("translation")]
        public FloatArrayType Translation { get; set; }

        [XmlElement("rotation")]
        public FloatArrayType Rotation { get; set; }

        [XmlElement("scale")]
        public FloatArrayType Scale { get; set; }

        [XmlElement(ElementName = "boundingbox")]
        public XcdBoundingBox BoundingBox { get; set; }

        [XmlElement(ElementName = "layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "customproperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
