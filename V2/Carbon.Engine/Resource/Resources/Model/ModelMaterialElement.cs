namespace Carbon.Engine.Resource.Resources.Model
{
    using Carbon.Engine.Logic;

    public class ModelMaterialElement : ResourceBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        public string DiffuseTexture { get; set; }
        public string NormalTexture { get; set; }
        public string AlphaTexture { get; set; }
        public string SpecularTexture { get; set; }

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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Name = source.ReadString();
            this.DiffuseTexture = source.ReadString();
            this.NormalTexture = source.ReadString();
            this.AlphaTexture = source.ReadString();
            this.SpecularTexture = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Name);
            target.Write(this.DiffuseTexture);
            target.Write(this.NormalTexture);
            target.Write(this.AlphaTexture);
            target.Write(this.SpecularTexture);
        }
    }
}
