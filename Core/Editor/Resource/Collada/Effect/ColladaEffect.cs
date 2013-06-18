﻿using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Effect
{
    [Serializable]
    public class ColladaEffect
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("profile_COMMON")]
        public ProfileCommon ProfileCommon { get; set; }
    }
}