using System;

namespace Carbon.Editor.Resource.Collada.Effect
{
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaEffectLibrary
    {
        [XmlElement(ElementName = "effect")]
        public ColladaEffect[] Effects { get; set; }
    }
}
