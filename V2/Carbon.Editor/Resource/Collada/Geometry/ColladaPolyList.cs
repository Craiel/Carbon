using System;

namespace Carbon.Editor.Resource.Collada.Geometry
{
    using System.Xml.Serialization;

    using Carbon.Editor.Resource.Collada.Data;

    [Serializable]
    public class ColladaPolyList : ColladaGeometryElement
    {
        [XmlElement(ElementName = "vcount")]
        public ColladaIntArrayType VertexCount { get; set; }
    }
}
