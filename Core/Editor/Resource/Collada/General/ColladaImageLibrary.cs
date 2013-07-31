using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.General
{
    [Serializable]
    public class ColladaImageLibrary
    {
        [XmlElement(ElementName = "image")]
        public ColladaImage[] Images { get; set; }
    }
}
