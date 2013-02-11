namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Font")]
    public class FontEntry : ContentEntry
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }
        
        [ContentEntryElement]
        public ContentLink Material { get; set; }
    }
}
