using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Content
{
    public enum ScriptType
    {
        Unknown = 0,
    }

    [ContentEntry("Script")]
    public class ScriptEntry : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int Id { get; set; }

        [ContentEntryElement]
        public ScriptType Type { get; set; }

        [ContentEntryElement]
        public ResourceLink? Script { get; set; }
    }
}
