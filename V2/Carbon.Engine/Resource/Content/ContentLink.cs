namespace Carbon.Engine.Resource.Content
{
    using Carbon.Engine.Contracts.Resource;

    [ContentEntry("ContentLink")]
    public struct ContentLink : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int ContentId { get; set; }

        [ContentEntryElement]
        public string Type { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }
    }
}
