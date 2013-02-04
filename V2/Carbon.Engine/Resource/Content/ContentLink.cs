namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("ContentLink")]
    public struct ContentLink
    {
        [ContentEntryElement]
        public int Id { get; set; }

        [ContentEntryElement]
        public int ContentId { get; set; }

        [ContentEntryElement]
        public string Type { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }
    }
}
