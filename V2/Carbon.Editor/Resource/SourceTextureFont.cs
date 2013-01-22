using System;
using System.Xml.Serialization;

using Carbon.Editor.Logic;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public class SourceTextureFont : SourceFolderContent
    {
        public SourceTextureFont()
        {
            this.FontSize = 10;
        }

        [XmlAttribute]
        public int FontSize { get; set; }

        [XmlElement]
        public SourceReference Font { get; set; }
        
        public override SourceFolderContent Clone()
        {
            SourceTextureFont clone = (SourceTextureFont)base.Clone();
            clone.Font = this.Font;
            clone.FontSize = this.FontSize;
            return clone;
        }

        public override void LoadFrom(SourceFolderContent source)
        {
            base.LoadFrom(source);

            var typedSource = (SourceTextureFont)source;
            this.Font = typedSource.Font;
            this.FontSize = typedSource.FontSize;
        }
    }
}
