using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    using Core.Editor.Resource.Collada.Data;

    [Serializable]
    public class EffectBump
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }
}
