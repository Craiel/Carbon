﻿using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdCustomProperties
    {
        [XmlElement(ElementName = "property")]
        public XcdProperty[] Properties { get; set; }
    }
}
