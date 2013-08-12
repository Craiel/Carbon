namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaTexture
    {
        [XmlAttribute("texture")]
        public string Texture { get; set; }

        [XmlAttribute("texcoord")]
        public string TexCoord { get; set; }
    }
}
