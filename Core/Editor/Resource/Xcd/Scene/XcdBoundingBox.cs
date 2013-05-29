using System;

namespace Core.Editor.Resource.Xcd.Scene
{
    using System.Xml.Serialization;

    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class XcdBoundingBox
    {
        [XmlElement("point")]
        public FloatArrayType[] Points { get; set; }
    }
}
