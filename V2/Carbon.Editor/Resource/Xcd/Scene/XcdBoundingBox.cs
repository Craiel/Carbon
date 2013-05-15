using System;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    using System.Xml.Serialization;

    using Carbon.Editor.Resource.Generic.Data;

    [Serializable]
    public class XcdBoundingBox
    {
        [XmlElement("point")]
        public FloatArrayType[] Points { get; set; }
    }
}
