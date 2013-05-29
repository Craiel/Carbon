using Core.Engine.Logic;

namespace Core.Engine.Rendering.RenderTarget
{
    public enum RenderTargetType
    {
        Default,
        Texture
    }

    public struct RenderTargetDescription
    {
        public RenderTargetType Type;

        public TypedVector2<int> Size;

        public int Index;

        public static RenderTargetDescription Texture(int index, TypedVector2<int> size)
        {
            return new RenderTargetDescription
                {
                    Type = RenderTargetType.Texture,
                    Size = size,
                    Index = index
                };
        }
    }
}
