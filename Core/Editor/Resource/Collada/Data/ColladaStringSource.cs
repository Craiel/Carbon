using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    [Serializable]
    public class ColladaStringSource
    {
        [XmlText]
        public string Content { get; set; }
    }
}
