namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaMatrix : FloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
