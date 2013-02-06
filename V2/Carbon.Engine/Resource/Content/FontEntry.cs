namespace Carbon.Engine.Resource.Content
{
    using Carbon.Engine.Contracts.Resource;

    [ContentEntry("Font")]
    public struct FontEntry : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int Size { get; set; }

        [ContentEntryElement]
        public ResourceLink Font { get; set; }
    }
}
