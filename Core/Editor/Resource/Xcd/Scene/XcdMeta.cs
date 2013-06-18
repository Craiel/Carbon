﻿using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdMeta
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("content")]
        public string Content { get; set; }
    }
}