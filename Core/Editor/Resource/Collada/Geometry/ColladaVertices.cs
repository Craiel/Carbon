namespace Core.Processing.Resource.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

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
