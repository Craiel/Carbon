using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public class SourceModel : SourceFolderContent
    {
        [XmlElement]
        public string File { get; set; }
        
        public override void LoadFrom(SourceFolderContent source)
        {
            SourceModel target = (SourceModel)source;
            this.File = target.File;
        }
    }
}
