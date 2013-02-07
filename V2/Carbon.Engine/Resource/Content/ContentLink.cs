﻿using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Content
{
    public enum ContentLinkType
    {
        Unknown = 0,
    }

    [ContentEntry("ContentLink")]
    public struct ContentLink : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int ContentId { get; set; }

        [ContentEntryElement]
        public ContentLinkType Type { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }
    }
}
