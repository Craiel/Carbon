using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Xcd
{
    using Core.Editor.Resource.Xcd.Scene;

    [Serializable]
    public class XcdHead
    {
        [XmlElement(ElementName = "meta")]
        public XcdMeta[] Metadata { get; set; }
    }
}
