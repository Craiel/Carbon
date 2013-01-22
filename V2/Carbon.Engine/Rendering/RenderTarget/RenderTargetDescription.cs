namespace Carbon.Engine.Rendering.RenderTarget
{
    public enum RenderTargetType
    {
        Default,
        Texture
    }

    public struct RenderTargetDescription
    {
        public RenderTargetType Type;

        public int Width;
        public int Height;

        public int Index;

        public static RenderTargetDescription Texture(int index, int width, int height)
        {
            return new RenderTargetDescription
                {
                    Type = RenderTargetType.Texture,
                    Width = width,
                    Height = height,
                    Index = index
                };
        }
    }
}
