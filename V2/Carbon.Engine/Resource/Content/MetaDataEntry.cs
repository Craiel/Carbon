namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("MetaData")]
    public class MetaDataEntry : ContentEntry
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ContentLink ContentId { get; set; }

        [ContentEntryElement]
        public string Key { get; set; }

        [ContentEntryElement]
        public string Value { get; set; }

        [ContentEntryElement]
        public int? ValueInt { get; set; }
    }
}
