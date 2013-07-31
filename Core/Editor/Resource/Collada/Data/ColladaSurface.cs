﻿using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    [Serializable]
    public class ColladaSurface
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "init_from")]
        public ColladaInitFrom InitFrom { get; set; }
    }
}
