using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectExtra
    {
        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }
    }
}
