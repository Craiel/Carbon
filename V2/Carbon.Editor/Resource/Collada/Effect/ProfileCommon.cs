using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class ProfileCommon
    {
        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }

        [XmlElement("extra")]
        public EffectExtra Extra { get; set; }
    }
}
