using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Xcd.Scene;

namespace Carbon.Editor.Resource.Xcd
{
    [Serializable]
    public class XcdScene
    {
        [XmlElement(ElementName = "Camera")]
        public XcdCamera[] Cameras { get; set; }

        [XmlElement(ElementName = "Light")]
        public XcdLight[] Lights { get; set; }

        [XmlElement(ElementName = "Mesh")]
        public XcdMesh[] Meshes { get; set; }
    }
}
