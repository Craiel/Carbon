using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Geometry
{
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class ColladaPolyList : ColladaGeometryElement
    {
        [XmlElement(ElementName = "vcount")]
        public IntArrayType VertexCount { get; set; }
    }
}
