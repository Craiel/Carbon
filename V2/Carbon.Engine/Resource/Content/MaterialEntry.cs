using SlimDX;

namespace Carbon.Engine.Resource.Content
{
    using Carbon.Engine.Contracts.Resource;

    [ContentEntry("Material")]
    public struct MaterialEntry : ICarbonContent
    {
        [ContentEntryElement]
        public Vector4 Color { get; set; }

        [ContentEntryElement]
        public ResourceLink? DiffuseTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink? NormalTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink? AlphaTexture { get; set; }

        [ContentEntryElement]
        public ResourceLink? SpecularTexture { get; set; }
    }
}
