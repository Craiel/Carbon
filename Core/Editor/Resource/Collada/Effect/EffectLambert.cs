using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectLambert
    {
        [XmlElement("emission")]
        public EffectEmission Emission { get; set; }

        [XmlElement("ambient")]
        public EffectAmbinet Ambient { get; set; }

        [XmlElement("diffuse")]
        public EffectDiffuse Diffuse { get; set; }

        [XmlElement("index_of_refraction")]
        public EffectIndexOfRefraction IndexOfRefraction { get; set; }
    }
}
