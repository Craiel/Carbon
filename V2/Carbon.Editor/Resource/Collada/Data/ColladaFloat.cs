using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Collada.Data
{
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
                this.Value = ColladaDataConversion.ConvertFloat(value)[0];
            }
        }

        [XmlIgnore]
        public float Value { get; private set; }
    }
}
