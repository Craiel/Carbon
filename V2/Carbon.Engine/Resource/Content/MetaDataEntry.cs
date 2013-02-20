namespace Carbon.Engine.Resource.Content
{
    public enum MetaDataKey
    {
        Unknown = 0,
        Name = 10,
    }

    [ContentEntry("MetaData")]
    public class MetaDataEntry : ContentEntry
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ContentLink ContentId { get; set; }

        [ContentEntryElement]
        public MetaDataKey Key { get; set; }

        [ContentEntryElement]
        public string Value { get; set; }

        [ContentEntryElement]
        public int? ValueInt { get; set; }
    }
}
