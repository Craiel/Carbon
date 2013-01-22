using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.Editor.Resource.Collada.Geometry
{
    using System.Xml.Serialization;

    using Carbon.Editor.Resource.Collada.Data;

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
        public ColladaIntArrayType P { get; set; }
    }
}
