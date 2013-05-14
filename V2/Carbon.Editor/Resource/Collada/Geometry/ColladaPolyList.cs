using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Collada.Geometry
{
    [Serializable]
    public class ColladaPolyList : ColladaGeometryElement
    {
        [XmlElement(ElementName = "vcount")]
        public IntArrayType VertexCount { get; set; }
    }
}
