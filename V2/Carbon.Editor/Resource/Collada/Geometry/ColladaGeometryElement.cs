using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;
using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Collada.Geometry
{
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
