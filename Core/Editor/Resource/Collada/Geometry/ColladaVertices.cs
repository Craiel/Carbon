using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Geometry
{
    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class ColladaVertices
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput Input { get; set; }
    }
}
