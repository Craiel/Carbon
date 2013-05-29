using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class InstanceEffect
    {
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
