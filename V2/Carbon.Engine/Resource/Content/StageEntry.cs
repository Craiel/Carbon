namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Stage")]
    public class StageEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public float SizeX { get; set; }
        [ContentEntryElement]
        public float SizeY { get; set; }
        [ContentEntryElement]
        public float SizeZ { get; set; }

        [ContentEntryElement]
        public ResourceLink StaticModel { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }
    }
}
