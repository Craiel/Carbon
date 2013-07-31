using System;
using System.Xml.Serialization;

namespace Core.Processing.Resource.Collada.Data
{
    using Core.Processing.Resource.Generic.Data;

    [Serializable]
    public class ColladaFloat
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
                this.Value = DataConversion.ConvertFloat(value)[0];
            }
        }

        [XmlIgnore]
        public float Value { get; private set; }
    }
}
