using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public abstract class SourceFolderContent
    {
        [XmlAttribute]
        public string Name { get; set; }

        public virtual SourceFolderContent Clone()
        {
            SourceFolderContent clone = (SourceFolderContent)Activator.CreateInstance(this.GetType());
            clone.LoadFrom(this);
            return clone;
        }

        public virtual void LoadFrom(SourceFolderContent source)
        {
            this.Name = source.Name;
        }
    }
}
