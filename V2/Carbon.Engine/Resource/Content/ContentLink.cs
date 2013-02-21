using System;

namespace Carbon.Engine.Resource.Content
{
    public enum ContentLinkType
    {
        Unknown = 0,
    }

    [ContentEntry("ContentLink")]
    public class ContentLink : ContentEntry 
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int ContentId { get; set; }

        [ContentEntryElement]
        public ContentLinkType Type { get; set; }

        [ContentEntryElement]
        public bool Mutable { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }
    }
}
