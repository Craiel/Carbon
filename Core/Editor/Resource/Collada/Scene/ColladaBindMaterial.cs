﻿using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaBindMaterial
    {
        [XmlElement("technique_common")]
        public ColladaBindTechnique Technique { get; set; }
    }
}
