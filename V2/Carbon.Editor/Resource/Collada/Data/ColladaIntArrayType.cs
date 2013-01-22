using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.Editor.Resource.Collada.Data
{
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaIntArrayType
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
                this.Data = ColladaDataConversion.ConvertInt(value);
            }
        }

        [XmlIgnore]
        public int[] Data { get; private set; }
    }
}
