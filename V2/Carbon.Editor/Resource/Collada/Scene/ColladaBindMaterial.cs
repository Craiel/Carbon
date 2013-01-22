using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaBindMaterial
    {
        [XmlElement("technique_common")]
        public ColladaBindTechnique Technique { get; set; }
    }
}
