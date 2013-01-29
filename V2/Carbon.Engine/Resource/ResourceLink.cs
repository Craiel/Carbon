namespace Carbon.Engine.Resource
{
    [ContentEntry("ResourceLink")]
    public struct ResourceLink
    {
        [ContentEntryElement]
        public int Id { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement]
        public string Source { get; set; }
    }
}
