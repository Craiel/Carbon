namespace Carbon.Engine.Resource.Content
{
    public enum MetaDataKey
    {
        Unknown = 0,
        Name = 10,
        SourcePath = 11,
        LastChangeDate = 12,
        ContentCount = 13,
        SourceElement = 14,
    }

    public enum MetaDataTargetEnum
    {
        Unknown = 0,
        Stage = 1,
        StageRegion = 2,
        ResourceTree = 3,
        Material = 4,
        Font = 5,
        Script = 6,
        Resource = 7,
        Project = 8,
    }

    [ContentEntry("MetaData")]
    public class MetaDataEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public MetaDataTargetEnum Target { get; set; }

        [ContentEntryElement]
        public int? TargetId { get; set; }

        [ContentEntryElement]
        public MetaDataKey Key { get; set; }

        [ContentEntryElement]
        public string Value { get; set; }

        [ContentEntryElement]
        public int? ValueInt { get; set; }

        [ContentEntryElement]
        public long? ValueLong { get; set; }

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
                return MetaDataTargetEnum.Unknown;
            }
        }
    }
}
