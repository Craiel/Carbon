using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Geometry
{
    using Core.Editor.Resource.Collada.Data;
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public abstract class ColladaGeometryElement
    {
        [XmlAttribute("material")]
        public string Material { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput[] Inputs { get; set; }

        // Todo: Rename this with something more reasonable
        [XmlElement(ElementName = "p")]
        public IntArrayType P { get; set; }
    }
}
