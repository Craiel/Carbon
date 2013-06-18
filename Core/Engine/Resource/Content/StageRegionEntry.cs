﻿namespace Core.Engine.Resource.Content
{
    [ContentEntry("StageRegion")]
    public class StageRegionEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ContentLink Playfield { get; set; }

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
                return MetaDataTargetEnum.StageRegion;
            }
        }
    }
}