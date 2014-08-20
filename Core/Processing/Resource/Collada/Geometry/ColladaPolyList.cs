namespace Core.Processing.Resource.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaPolyList : ColladaGeometryElement
    {
        [XmlElement(ElementName = "vcount")]
        public IntArrayType VertexCount { get; set; }
    }
}
