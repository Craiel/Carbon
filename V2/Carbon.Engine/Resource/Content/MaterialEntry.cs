using SlimDX;

namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Material")]
    public struct MaterialContent
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
