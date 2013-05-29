namespace Core.Engine.Resource.Content
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
        public ContentLink StaticModel { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }

        public override MetaDataTargetEnum MetaDataTarget
        {
            get
            {
                return MetaDataTargetEnum.Stage;
            }
        }
    }
}
