namespace Core.Processing.Resource.Collada.General
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaImageLibrary
    {
        [XmlElement(ElementName = "image")]
        public ColladaImage[] Images { get; set; }
    }
}
