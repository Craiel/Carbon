using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.General
{
    using Core.Editor.Resource.Collada.Data;

    [Serializable]
    public class ColladaImage
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "init_from")]
        public ColladaInitFrom InitFrom { get; set; }
    }
}
