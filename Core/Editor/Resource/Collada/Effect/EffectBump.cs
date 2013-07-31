using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Effect
{
    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class EffectBump
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }
}
