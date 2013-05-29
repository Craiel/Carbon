using System;
using System.Xml.Serialization;

using SlimDX;

namespace Core.Editor.Resource.Collada.Data
{
    using Core.Editor.Resource.Generic.Data;

    [Serializable]
    public class ColladaColor
    {
        [XmlAttribute("sid")]
        public string SID { get; set; }

        [XmlText]
        public string RawData
        {
            get
            {
                return string.Empty;
            }

            set
            {
                float[] data = DataConversion.ConvertFloat(value);
                this.Color = new Vector4(data[0], data[1], data[2], data[3]);
            }
        }

        [XmlIgnore]
        public Vector4 Color { get; private set; }
    }
}
