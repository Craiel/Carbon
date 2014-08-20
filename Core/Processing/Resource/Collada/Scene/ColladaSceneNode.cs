namespace Core.Processing.Resource.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class ColladaSceneNode
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("translate")]
        public ColladaTranslate Translation { get; set; }

        [XmlElement("rotate")]
        public ColladaRotate[] Rotations { get; set; }

        [XmlElement("scale")]
        public ColladaScale Scale { get; set; }

        [XmlElement("matrix")]
        public ColladaMatrix[] Matrices { get; set; }

        [XmlElement("instance_geometry")]
        public ColladaInstanceGeometry InstanceGeometry { get; set; }

        [XmlElement("node")]
        public ColladaSceneNode[] Children { get; set; }
    }
}
