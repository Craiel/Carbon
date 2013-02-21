namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("ResourceLink")]
    public class ResourceLink : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement]
        public string Path { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }
    }
}
