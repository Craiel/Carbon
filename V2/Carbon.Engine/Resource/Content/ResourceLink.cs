namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("ResourceLink")]
    public class ResourceLink : ContentEntry
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement]
        public string Source { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }
    }
}
