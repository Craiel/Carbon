namespace Core.Processing.Resource.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaBindMaterial
    {
        [XmlElement("technique_common")]
        public ColladaBindTechnique Technique { get; set; }
    }
}
