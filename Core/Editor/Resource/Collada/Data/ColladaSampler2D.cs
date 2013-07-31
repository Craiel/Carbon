using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    [Serializable]
    public class ColladaSampler2D
    {
        [XmlElement(ElementName = "source")]
        public ColladaStringSource Source { get; set; }
    }
}
