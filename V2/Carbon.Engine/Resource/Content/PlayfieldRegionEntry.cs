using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("PlayfieldRegion")]
    public class PlayfieldRegionEntry : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int Id { get; set; }

        [ContentEntryElement]
        public ContentLink? Playfield { get; set; }

        [ContentEntryElement]
        public float PositionX { get; set; }
        [ContentEntryElement]
        public float PositionY { get; set; }
        [ContentEntryElement]
        public float PositionZ { get; set; }

        [ContentEntryElement]
        public float SizeX { get; set; }
        [ContentEntryElement]
        public float SizeY { get; set; }
        [ContentEntryElement]
        public float SizeZ { get; set; }
    }
}
