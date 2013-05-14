using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Generic.Data
{
    [Serializable]
    public class IntArrayType
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
                this.Data = DataConversion.ConvertInt(value);
            }
        }

        [XmlIgnore]
        public int[] Data { get; private set; }
    }
}
