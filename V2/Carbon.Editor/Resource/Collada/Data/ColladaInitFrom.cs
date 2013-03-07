using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaInitFrom
    {
        [XmlText]
        public string Source { get; set; }
    }
}
