using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Xcd
{
    using Core.Processing.Resource.Xcd.Scene;

    [Serializable]
    public class XcdHead
    {
        [XmlElement(ElementName = "meta")]
        public XcdMeta[] Metadata { get; set; }
    }
}
