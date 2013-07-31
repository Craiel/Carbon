using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaBindInstanceMaterial
    {
        [XmlAttribute("symbol")]
        public string Symbol { get; set; }

        [XmlAttribute("target")]
        public string Target { get; set; }

        [XmlElement("bind_vertex_input")]
        public ColladaBindVertexInput VertexInput { get; set; }
    }
}
