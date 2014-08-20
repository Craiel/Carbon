﻿namespace Core.Processing.Resource.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    using Core.Processing.Resource.Generic.Data;

    using SharpDX;

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
                this.Value = new Vector4(data[0], data[1], data[2], data[3]);
            }
        }

        [XmlIgnore]
        public Vector4 Value { get; private set; }
    }
}