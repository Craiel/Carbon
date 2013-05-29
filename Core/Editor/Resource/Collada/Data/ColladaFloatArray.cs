using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Data
{
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class ColladaFloatArray : FloatArrayType
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }
    }
}