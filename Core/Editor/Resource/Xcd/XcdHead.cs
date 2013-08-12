namespace Core.Processing.Resource.Xcd
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Xcd.Scene;

    [Serializable]
    public class XcdHead
    {
        [XmlElement(ElementName = "meta")]
        public XcdMeta[] Metadata { get; set; }
    }
}
