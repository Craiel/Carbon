using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Content
{
    [ContentEntry("Material")]
    public struct MaterialEntry : ICarbonContent
    {
        [ContentEntryElement(PrimaryKey = PrimaryKeyMode.AutoIncrement)]
        public int Id { get; set; }

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
