using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Effect
{
    [Serializable]
    public class InstanceEffect
    {
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
