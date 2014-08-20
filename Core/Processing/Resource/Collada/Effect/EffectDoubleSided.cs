namespace Core.Processing.Resource.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class EffectDoubleSided
    {
        [XmlText]
        public string DoubleSided { get; set; }
    }
}
