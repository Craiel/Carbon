using System;
using System.Xml.Serialization;

using Carbon.Editor.Resource.Generic.Data;

namespace Carbon.Editor.Resource.Xcd.Scene
{
    [Serializable]
    public class XcdLight
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("radius")]
        public float Radius { get; set; }

        [XmlAttribute("intensity")]
        public float Intensity { get; set; }

        [XmlAttribute("ambientIntensity")]
        public float AmbientIntensity { get; set; }

        [XmlAttribute("beamWidth")]
        public float BeamWidth { get; set; }

        [XmlAttribute("cutOffAngle")]
        public float CutoffAngle { get; set; }

        [XmlAttribute("location")]
        public FloatArrayAttribute Location { get; set; }

        [XmlAttribute("direction")]
        public FloatArrayAttribute Direction { get; set; }

        [XmlAttribute("color")]
        public FloatArrayAttribute Color { get; set; }

        [XmlElement(ElementName = "Layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "CustomProperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
