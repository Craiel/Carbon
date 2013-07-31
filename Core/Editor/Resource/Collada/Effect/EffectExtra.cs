using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Effect
{
    [Serializable]
    public class EffectExtra
    {
        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }
    }
}
