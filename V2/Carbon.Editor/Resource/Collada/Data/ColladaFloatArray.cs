using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaFloatArray : FloatArrayType
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }
    }
}