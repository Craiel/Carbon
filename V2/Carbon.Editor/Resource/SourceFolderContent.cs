using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource
{
    [Serializable]
    [XmlInclude(typeof(SourceTextureFont))]
    [XmlInclude(typeof(SourceModel))]
    public abstract class SourceFolderContent
    {
        [XmlAttribute]
        public string Name { get; set; }

        public virtual SourceFolderContent Clone()
        {
            SourceFolderContent clone = (SourceFolderContent)Activator.CreateInstance(this.GetType());
            clone.Name = this.Name;
            return clone;
        }

        public virtual void LoadFrom(SourceFolderContent source)
        {
            this.Name = source.Name;
        }
    }
}
