using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Data
{
    [Serializable]
    public class ColladaInitFrom
    {
        [XmlText]
        public string Source { get; set; }
    }
}
