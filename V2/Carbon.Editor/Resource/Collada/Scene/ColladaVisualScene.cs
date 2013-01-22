using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaVisualScene
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("node")]
        public ColladaSceneNode[] Nodes { get; set; }
    }
}
