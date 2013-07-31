using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Geometry
{
    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaPolyList : ColladaGeometryElement
    {
        [XmlElement(ElementName = "vcount")]
        public IntArrayType VertexCount { get; set; }
    }
}
