﻿using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Generic.Data
{
    [Serializable]
    public class BoolArrayType
    {
        [XmlText]
        public string RawData
        {
            get
            {
                return string.Empty;
            }

            set
            {
                this.Data = DataConversion.ConvertBool(value);
            }
        }

        [XmlIgnore]
        public bool[] Data { get; private set; }
    }
}
