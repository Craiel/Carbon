using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectEmission
    {
        [XmlElement("color")]
        public ColladaColor Color { get; set; }
    }

    [Serializable]
    public class EffectAmbinet
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }

    [Serializable]
    public class EffectDiffuse
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }

    [Serializable]
    public class EffectSpecular
    {
        [XmlElement("color")]
        public ColladaColor Color { get; set; }
    }

    [Serializable]
    public class EffectShininess
    {
        [XmlElement("float")]
        public ColladaFloat Float { get; set; }
    }

    [Serializable]
    public class EffectTransparency
    {
        [XmlElement("float")]
        public ColladaFloat Float { get; set; }
    }

    [Serializable]
    public class EffectIndexOfRefraction
    {
        [XmlElement("float")]
        public ColladaFloat Float { get; set; }
    }

    [Serializable]
    public class EffectTransparent
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; } 
    }
}
