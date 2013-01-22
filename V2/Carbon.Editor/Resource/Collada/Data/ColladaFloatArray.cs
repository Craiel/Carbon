using System;

namespace Carbon.Editor.Resource.Collada.Data
{
    using System.Xml.Serialization;
    
    [Serializable]
    public class ColladaFloatArray : ColladaFloatArrayType
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }
    }
}