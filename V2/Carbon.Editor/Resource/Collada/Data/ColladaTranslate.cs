using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaTranslate : ColladaFloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
