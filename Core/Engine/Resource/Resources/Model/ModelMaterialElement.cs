namespace Core.Engine.Resource.Resources.Model
{
    public class ModelMaterialElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelMaterialElement()
        {
        }

        public ModelMaterialElement(Protocol.Resource.ModelMaterial data)
            : this()
        {
            this.Name = data.Name;
            this.DiffuseTexture = data.DiffuseTexture;
            this.NormalTexture = data.NormalTexture;
            this.AlphaTexture = data.AlphaTexture;
            this.SpecularTexture = data.SpecularTexture;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        public string DiffuseTexture { get; set; }
        public string NormalTexture { get; set; }
        public string AlphaTexture { get; set; }
        public string SpecularTexture { get; set; }

        public Protocol.Resource.ModelMaterial.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.ModelMaterial.Builder { Name = this.Name };
            if (!string.IsNullOrEmpty(this.DiffuseTexture))
            {
                builder.DiffuseTexture = this.DiffuseTexture;
            }

            if (!string.IsNullOrEmpty(this.NormalTexture))
            {
                builder.NormalTexture = this.NormalTexture;
            }

            if (!string.IsNullOrEmpty(this.AlphaTexture))
            {
                builder.AlphaTexture = this.AlphaTexture;
            }

            if (!string.IsNullOrEmpty(this.SpecularTexture))
            {
                builder.SpecularTexture = this.SpecularTexture;
            }

            return builder;
        }

        public ModelMaterialElement Clone()
        {
            return new ModelMaterialElement
            {
                Name = this.Name,
                DiffuseTexture = this.DiffuseTexture,
                NormalTexture = this.NormalTexture,
                AlphaTexture = this.AlphaTexture,
                SpecularTexture = this.SpecularTexture
            };
        }
    }
}
