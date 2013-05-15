using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Xcd.Scene;

namespace Carbon.Editor.Resource.Xcd
{
    [Serializable]
    public class XcdScene
    {
        [XmlElement(ElementName = "camera")]
        public XcdCamera[] Cameras { get; set; }

        [XmlElement(ElementName = "light")]
        public XcdLight[] Lights { get; set; }

        [XmlElement(ElementName = "mesh")]
        public XcdMesh[] Meshes { get; set; }
    }
}
