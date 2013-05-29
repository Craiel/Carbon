namespace Core.Engine.Resource.Content
{
    public enum ScriptType
    {
        Unknown = 0,
    }

    [ContentEntry("Script")]
    public class ScriptEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ScriptType Type { get; set; }

        [ContentEntryElement]
        public ContentLink Script { get; set; }

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
                return MetaDataTargetEnum.Script;
            }
        }
    }
}
