using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;

namespace Carbon.Editor.Resource.Collada.General
{
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
