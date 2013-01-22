using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaRotate : ColladaFloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
