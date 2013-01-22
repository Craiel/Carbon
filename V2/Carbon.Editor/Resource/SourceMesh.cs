using System;
using System.Xml.Serialization;

using Carbon.Editor.Logic;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public class SourceMesh : SourceFolderContent
    {
        [XmlAttribute]
        public byte[] SourceMd5 { get; set; }

        [XmlElement]
        public SourceReference SourceFile { get; set; }
        
        public override SourceFolderContent Clone()
        {
            SourceModel clone = (SourceModel)base.Clone();
            clone.SourceFile = this.SourceFile;
            return clone;
        }

        public override void LoadFrom(SourceFolderContent source)
        {
            base.LoadFrom(source);

            var typedSource = (SourceModel)source;
            this.SourceFile = typedSource.SourceFile;
        }
    }
}
