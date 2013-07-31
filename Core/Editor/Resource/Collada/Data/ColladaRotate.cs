using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaRotate : FloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
