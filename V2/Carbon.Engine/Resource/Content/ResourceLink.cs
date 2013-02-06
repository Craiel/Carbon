﻿namespace Carbon.Engine.Resource.Content
{
    using Carbon.Engine.Contracts.Resource;

    [ContentEntry("ResourceLink")]
    public struct ResourceLink : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement]
        public string Source { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }
    }
}
