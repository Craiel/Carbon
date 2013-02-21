namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("ResourceTree")]
    public class ResourceTree : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ContentLink Parent { get; set; }

        [ContentEntryElement]
        public string Name { get; set; }

        [ContentEntryElement]
        public string FullPath { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }
    }
}
