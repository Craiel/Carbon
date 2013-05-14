using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaTranslate : FloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
