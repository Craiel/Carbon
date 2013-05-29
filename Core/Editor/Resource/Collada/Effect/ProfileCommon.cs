using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class ProfileCommon
    {
        [XmlElement("newparam")]
        public EffectParameter[] Parameter { get; set; }

        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }

        [XmlElement("extra")]
        public EffectExtra Extra { get; set; }
    }
}
