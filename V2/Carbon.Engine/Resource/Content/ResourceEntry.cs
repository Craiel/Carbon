namespace Carbon.Engine.Resource.Content
{
    public enum ResourceType
    {
        Unknown = 0,
        Raw = 10,
        Mesh = 11,
        Texture = 12,
        Font = 13,
    }

    [ContentEntry("Resource")]
    public class ResourceEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public ResourceType Type { get; set; }

        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement]
        public ContentLink TreeNode { get; set; }

        [ContentEntryElement]
        public byte[] Md5 { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }

        public override Contracts.Resource.ICarbonContent Clone(bool fullCopy = false)
        {
            var clone = new ResourceEntry();
            clone.LoadFrom(this);
            if (fullCopy)
            {
                clone.Id = this.Id;
                clone.Hash = this.Hash;
                clone.Md5 = this.Md5;
                if (this.TreeNode != null)
                {
                    clone.TreeNode.Id = this.TreeNode.Id;
                }
            }

            return clone;
        }

        public override void LoadFrom(Contracts.Resource.ICarbonContent source)
        {
            var other = source as ResourceEntry;
            this.Type = other.Type;
            if (other.TreeNode != null)
            {
                this.TreeNode = new ContentLink();
                this.TreeNode.LoadFrom(other.TreeNode);
            }
        }
    }
}
