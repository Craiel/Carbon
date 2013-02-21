namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Material")]
    public class MaterialEntry : ContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int? Id { get; set; }

        [ContentEntryElement]
        public float ColorR { get; set; }

        [ContentEntryElement]
        public float ColorG { get; set; }

        [ContentEntryElement]
        public float ColorB { get; set; }

        [ContentEntryElement]
        public float ColorA { get; set; }

        [ContentEntryElement]
        public ResourceLink DiffuseTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink NormalTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink AlphaTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink SpecularTexture { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.Id == null;
            }
        }

        public override Contracts.Resource.ICarbonContent Clone(bool fullCopy = false)
        {
            var clone = new MaterialEntry();
            clone.LoadFrom(this);
            if (fullCopy)
            {
                clone.Id = this.Id;
                if (clone.DiffuseTexture != null)
                {
                    clone.DiffuseTexture.Id = this.DiffuseTexture.Id;
                }

                if (clone.NormalTexture != null)
                {
                    clone.NormalTexture.Id = this.NormalTexture.Id;
                }

                if (clone.SpecularTexture != null)
                {
                    clone.SpecularTexture.Id = this.SpecularTexture.Id;
                }

                if (clone.AlphaTexture != null)
                {
                    clone.AlphaTexture.Id = this.AlphaTexture.Id;
                }
            }

            return clone;
        }

        public override void LoadFrom(Contracts.Resource.ICarbonContent source)
        {
            var other = source as MaterialEntry;
            this.ColorR = other.ColorR;
            this.ColorG = other.ColorG;
            this.ColorB = other.ColorB;
            this.ColorA = other.ColorA;
            if (other.DiffuseTexture != null)
            {
                this.DiffuseTexture = new ResourceLink();
                this.DiffuseTexture.LoadFrom(other.DiffuseTexture);
            }

            if (other.NormalTexture != null)
            {
                this.NormalTexture = new ResourceLink();
                this.NormalTexture.LoadFrom(other.NormalTexture);
            }

            if (other.SpecularTexture != null)
            {
                this.SpecularTexture = new ResourceLink();
                this.SpecularTexture.LoadFrom(other.SpecularTexture);
            }

            if (other.AlphaTexture != null)
            {
                this.AlphaTexture = new ResourceLink();
                this.AlphaTexture.LoadFrom(other.AlphaTexture);
            }
        }
    }
}
