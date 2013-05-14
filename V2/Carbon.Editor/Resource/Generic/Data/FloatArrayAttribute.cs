using System;
using System.Xml;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource.Generic.Data
{
    [Serializable]
    public class FloatArrayAttribute : XmlAttribute
    {
        protected internal FloatArrayAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc)
            : base(prefix, localName, namespaceURI, doc)
        {
        }

        public override string Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value;
                this.Data = DataConversion.ConvertFloat(value);
            }
        }
        
        [XmlIgnore]
        public float[] Data { get; private set; }
    }
}
