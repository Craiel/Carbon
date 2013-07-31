﻿using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdProperty
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }
    }
}
