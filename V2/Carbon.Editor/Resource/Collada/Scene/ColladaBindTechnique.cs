using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaBindTechnique
    {
        [XmlElement("instance_material")]
        public ColladaBindInstanceMaterial InstanceMaterial { get; set; }
    }
}
