using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public class SourceProjectFolder : SourceFolderContent
    {
        public SourceProjectFolder()
        {
            this.Contents = new List<SourceFolderContent>();
        }

        [XmlElement]
        public List<SourceFolderContent> Contents { get; set; }

        public override SourceFolderContent Clone()
        {
            SourceProjectFolder clone = (SourceProjectFolder)base.Clone();
            foreach (SourceFolderContent sourceFolderContent in Contents)
            {
                clone.Contents.Add(sourceFolderContent.Clone());
            }
            return clone;
        }
    }
}
