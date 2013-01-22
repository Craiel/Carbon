using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Geometry
{
    using Carbon.Editor.Resource.Collada.Data;

    [Serializable]
    public class ColladaVertices
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput Input { get; set; }
    }
}
