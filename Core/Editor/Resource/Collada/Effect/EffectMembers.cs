namespace Core.Processing.Resource.Collada.Effect
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Collada.Data;

    [Serializable]
    public class EffectEmission
    {
        [XmlElement("color")]
        public ColladaColor Color { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here."),
    Serializable]
    public class EffectAmbient
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
