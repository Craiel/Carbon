using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaTexture
    {
        [XmlAttribute("texture")]
        public string Texture { get; set; }

        [XmlAttribute("texcoord")]
        public string TexCoord { get; set; }
    }
}
