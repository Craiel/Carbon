using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdTransform
    {
        [XmlAttribute("translation")]
        public FloatArrayAttribute Translation { get; set; }

        [XmlAttribute("rotation")]
        public FloatArrayAttribute Rotation { get; set; }

        [XmlAttribute("scale")]
        public FloatArrayAttribute Scale { get; set; }
    }
}
