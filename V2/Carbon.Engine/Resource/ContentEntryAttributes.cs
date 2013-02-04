using System;

namespace Carbon.Engine.Resource
{
    public enum PrimaryKeyMode
    {
        None,
        Assigned,
        AutoIncrement
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class ContentEntryAttribute : Attribute
    {
        public ContentEntryAttribute(string table)
        {
            this.Table = table;
        }

        public string Table { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ContentEntryElementAttribute : Attribute
    {
        public ContentEntryElementAttribute()
        {
        }

        public ContentEntryElementAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public PrimaryKeyMode PrimaryKey { get; set; }
    }
}