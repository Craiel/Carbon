namespace Carbon.Engine.Resource.Content
{
    public enum MetaDataKey
    {
        Unknown = 0,
        Name = 10,
        SourcePath = 11,
        LastChangeDate = 12,
        ContentCount = 13,
    }

    public enum MetaDataTarget
    {
        Unknown = 0,
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
        public MetaDataTarget TargetType { get; set; }

        [ContentEntryElement]
        public int? TargetId { get; set; }

        [ContentEntryElement]
        public MetaDataKey Key { get; set; }

        [ContentEntryElement]
        public string Value { get; set; }

        [ContentEntryElement]
        public int? ValueInt { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }
    }
}
