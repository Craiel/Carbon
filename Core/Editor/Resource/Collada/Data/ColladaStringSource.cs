namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaStringSource
    {
        [XmlText]
        public string Content { get; set; }
    }
}
