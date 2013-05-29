using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectPhong
    {
        [XmlElement("emission")]
        public EffectEmission Emission { get; set; }

        [XmlElement("ambient")]
        public EffectAmbinet Ambient { get; set; }

        [XmlElement("diffuse")]
        public EffectDiffuse Diffuse { get; set; }

        [XmlElement("specular")]
        public EffectSpecular Specular { get; set; }

        [XmlElement("shininess")]
        public EffectShininess Shininess { get; set; }

        [XmlElement("transparency")]
        public EffectTransparency Transparency { get; set; }

        [XmlElement("transparent")]
        public EffectTransparent Transparent { get; set; }
    }
}
