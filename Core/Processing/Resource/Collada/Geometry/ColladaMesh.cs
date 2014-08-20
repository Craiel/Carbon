namespace Core.Processing.Resource.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class ColladaMesh
    {
        [XmlElement(ElementName = "source")]
        public ColladaSource[] Sources { get; set; }

        [XmlElement(ElementName = "vertices")]
        public ColladaVertices Vertices { get; set; }

        [XmlElement(ElementName = "polylist")]
        public ColladaPolyList[] PolyLists { get; set; }
    }
}
