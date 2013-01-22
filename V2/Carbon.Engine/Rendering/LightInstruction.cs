using Carbon.Engine.Contracts.Rendering;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public sealed class LightInstruction
    {
        public ILight Light { get; set; }

        public Vector4 Position { get; set; }

        public Matrix World { get; set; }
    }
}
