using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaInstanceGeometry
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlElement("bind_material")]
        public ColladaBindMaterial BindMaterial { get; set; }
    }
}
