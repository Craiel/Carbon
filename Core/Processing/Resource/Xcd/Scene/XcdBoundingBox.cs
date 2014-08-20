namespace Core.Processing.Resource.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class XcdBoundingBox
    {
        [XmlElement("point")]
        public FloatArrayType[] Points { get; set; }
    }
}
