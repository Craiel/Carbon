﻿using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class EffectBlinn
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

        [XmlElement("index_of_refraction")]
        public EffectIndexOfRefraction IndexOfRefraction { get; set; }
    }
}
