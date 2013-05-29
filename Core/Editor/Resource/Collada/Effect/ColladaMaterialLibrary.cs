using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class ColladaMaterialLibrary
    {
        [XmlElement("material")]
        public ColladaMaterial[] Materials { get; set; }
    }
}
