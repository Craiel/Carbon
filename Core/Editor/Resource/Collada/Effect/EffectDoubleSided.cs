using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectDoubleSided
    {
        [XmlText]
        public string DoubleSided { get; set; }
    }
}
