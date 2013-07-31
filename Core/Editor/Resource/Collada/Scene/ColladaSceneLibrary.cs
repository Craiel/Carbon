﻿using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Scene
{
    [Serializable]
    public class ColladaSceneLibrary
    {
        [XmlElement("visual_scene")]
        public ColladaVisualScene VisualScene { get; set; }
    }
}
