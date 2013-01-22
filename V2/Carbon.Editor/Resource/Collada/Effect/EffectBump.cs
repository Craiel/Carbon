using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectBump
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }
}
