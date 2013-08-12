namespace Core.Engine.Resource.Content
{
    using System;

    [ContentEntry("Font")]
    public class FontEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int CharactersPerRow { get; set; }
        
        [ContentEntryElement]
        public ContentLink Resource { get; set; }

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
                return MetaDataTargetEnum.Font;
            }
        }

        public override Contracts.Resource.ICarbonContent Clone(bool fullCopy = false)
        {
            var clone = new FontEntry();
            clone.LoadFrom(this);
            if (fullCopy)
            {
                clone.Id = this.Id;
                if (clone.Resource != null)
                {
                    clone.Resource.Id = this.Resource.Id;
                }
            }

            return clone;
        }

        public override void LoadFrom(Contracts.Resource.ICarbonContent source)
        {
            var other = source as FontEntry;
            if (other == null)
            {
                throw new ArgumentException("Argument is null or invalid type");
            }

            this.CharactersPerRow = other.CharactersPerRow;

            if (other.Resource != null)
            {
                this.Resource = new ContentLink();
                this.Resource.LoadFrom(other.Resource);
            }
        }
    }
}
