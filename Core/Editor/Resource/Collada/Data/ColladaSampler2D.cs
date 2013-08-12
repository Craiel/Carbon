namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaSampler2D
    {
        [XmlElement(ElementName = "source")]
        public ColladaStringSource Source { get; set; }
    }
}
