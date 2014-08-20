namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaInitFrom
    {
        [XmlText]
        public string Source { get; set; }
    }
}
