namespace Core.Processing.Resource.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class EffectBump
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }
}
