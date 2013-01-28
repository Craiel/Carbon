namespace Carbon.Engine.Resource
{
    [ContentEntry("ResourceLink")]
    public struct ResourceLink
    {
        [ContentEntryElement]
        public int Id { get; set; }

        [ContentEntryElement]
        public byte[] Hash { get; set; }

        [ContentEntryElement]
        public string Source { get; set; }
    }
}
