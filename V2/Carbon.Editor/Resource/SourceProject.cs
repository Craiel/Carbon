using System;
using System.Xml.Serialization;

namespace Carbon.Editor.Resource
{
    [Serializable]
    public class SourceProject
    {
        public SourceProject()
        {
            this.Root = new SourceProjectFolder { Name = "Root" };
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public SourceProjectFolder Root { get; set; }

        public virtual SourceProject Clone()
        {
            SourceProject clone = (SourceProject)Activator.CreateInstance(this.GetType());
            clone.Name = this.Name;
            clone.Root = (SourceProjectFolder)this.Root.Clone();
            return clone;
        }

        public virtual void LoadFrom(SourceProject source)
        {
            this.Name = source.Name;
            this.Root = source.Root;
        }
    }
}
