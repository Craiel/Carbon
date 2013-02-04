using System;

namespace Carbon.Engine.Resource
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class ContentEntryAttribute : Attribute
    {
        public ContentEntryAttribute(string table)
        {
            this.Table = table;
        }

        public string Table { get; private set; }
    }

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

        public bool PrimaryKey { get; set; }
    }
}