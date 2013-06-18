﻿using System;
using System.Xml.Serialization;

namespace Core.Editor.Resource.Collada.Data
{
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class ColladaScale : FloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}