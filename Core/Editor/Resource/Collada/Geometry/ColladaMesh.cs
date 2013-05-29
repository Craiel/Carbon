using System;

namespace Core.Editor.Resource.Collada.Geometry
{
    using System.Xml.Serialization;

    using Core.Editor.Resource.Collada.Data;

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
