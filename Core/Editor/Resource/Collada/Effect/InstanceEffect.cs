namespace Core.Processing.Resource.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class InstanceEffect
    {
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
