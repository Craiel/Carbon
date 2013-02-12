﻿namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Playfield")]
    public class PlayfieldEntry : ContentEntry
    {
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
    }
}