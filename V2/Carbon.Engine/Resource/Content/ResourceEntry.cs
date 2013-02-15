using System;

namespace Carbon.Engine.Resource.Content
{
    public enum ResourceType
    {
        Unknown = 0,
        Raw = 10,
        Mesh = 11,
        Texture = 12,
        Font = 13,
    }

    [ContentEntry("Resource")]
    public class ResourceEntry : ContentEntry
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ResourceType Type { get; set; }

        [ContentEntryElement]
        public string SourcePath { get; set; }

        [ContentEntryElement]
        public byte[] Md5 { get; set; }

        [ContentEntryElement]
        public DateTime LastRefreshTime { get; set; }

        [ContentEntryElement]
        public ResourceLink Resource { get; set; }
    }
}
