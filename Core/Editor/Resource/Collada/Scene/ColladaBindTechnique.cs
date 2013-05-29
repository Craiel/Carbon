using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaBindTechnique
    {
        [XmlElement("instance_material")]
        public ColladaBindInstanceMaterial InstanceMaterial { get; set; }
    }
}
