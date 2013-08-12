namespace Core.Processing.Resource.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class EffectExtra
    {
        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }
    }
}
