using System;

namespace Carbon.Editor.Resource.Collada.Geometry
{
    using System.Xml.Serialization;

    using Carbon.Editor.Resource.Collada.Data;

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
