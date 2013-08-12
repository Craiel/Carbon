namespace Core.Processing.Resource.Xcd
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Xcd.Scene;

    [Serializable]
    public class XcdScene
    {
        [XmlElement(ElementName = "camera")]
        public XcdCamera[] Cameras { get; set; }

        [XmlElement(ElementName = "light")]
        public XcdLight[] Lights { get; set; }

        [XmlElement(ElementName = "mesh")]
        public XcdMesh[] Meshes { get; set; }

        [XmlElement(ElementName = "element")]
        public XcdElement[] Elements { get; set; }
    }
}
