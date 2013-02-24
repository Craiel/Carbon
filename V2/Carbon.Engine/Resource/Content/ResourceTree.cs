namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("ResourceTree")]
    public class ResourceTree : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public int? Parent { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }

        public override Contracts.Resource.ICarbonContent Clone(bool fullCopy = false)
        {
            var clone = new ResourceTree();
            clone.LoadFrom(this);
            if (fullCopy)
            {
                clone.Id = this.Id;
                clone.Hash = this.Hash;
            }

            return clone;
        }

        public override void LoadFrom(Contracts.Resource.ICarbonContent source)
        {
            var other = source as ResourceTree;
            this.Parent = other.Parent;
        }
    }
}
