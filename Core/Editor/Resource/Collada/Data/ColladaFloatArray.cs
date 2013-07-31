using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaFloatArray : FloatArrayType
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }
    }
}