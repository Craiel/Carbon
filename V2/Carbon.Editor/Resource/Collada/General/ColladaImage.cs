using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Collada.Data;

namespace Carbon.Editor.Resource.Collada.General
{
    [Serializable]
    public class ColladaImage
    {
        [XmlElement(ElementName = "init_from")]
        public ColladaInitFrom InitFrom { get; set; }
    }
}
