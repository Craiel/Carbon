namespace Core.Engine.Resource.Content
{
    using System;

    public enum ContentLinkType
    {
        Unknown = 0,
        ResourceTreeNode = 1,
        Resource = 2,
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
        public int? ContentId { get; set; }

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

        public override MetaDataTargetEnum MetaDataTarget
        {
            get
            {
                return MetaDataTargetEnum.Unknown;
            }
        }

        public override void LoadFrom(Contracts.Resource.ICarbonContent source)
        {
            var other = source as ContentLink;
            if (other == null)
            {
                throw new ArgumentException("Argument is null or invalid type");
            }

            this.ContentId = other.ContentId;
            this.Type = other.Type;
            this.Mutable = other.Mutable;
        }

        public override Contracts.Resource.ICarbonContent Clone(bool fullCopy = false)
        {
            var clone = new ContentLink();
            clone.LoadFrom(this);
            return clone;
        }
    }
}
