using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Xcd.Scene;

namespace Carbon.Editor.Resource.Xcd
{
    [Serializable]
    public class XcdHead
    {
        [XmlElement(ElementName = "meta")]
        public XcdMeta[] Metadata { get; set; }
    }
}
