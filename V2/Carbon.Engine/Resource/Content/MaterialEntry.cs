using SlimDX;

namespace Carbon.Engine.Resource.Content
{
    using Carbon.Engine.Contracts.Resource;

    [ContentEntry("Material")]
    public struct MaterialEntry : ICarbonContent
    {
        [ContentEntryElement]
        public float ColorR { get; set; }

        [ContentEntryElement]
        public float ColorG { get; set; }

        [ContentEntryElement]
        public float ColorB { get; set; }

        [ContentEntryElement]
        public float ColorA { get; set; }

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
