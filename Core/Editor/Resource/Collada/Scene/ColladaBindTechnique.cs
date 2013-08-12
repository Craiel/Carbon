namespace Core.Processing.Resource.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaBindTechnique
    {
        [XmlElement("instance_material")]
        public ColladaBindInstanceMaterial InstanceMaterial { get; set; }
    }
}
