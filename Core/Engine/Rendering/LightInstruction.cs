namespace Core.Engine.Rendering
{
    using Core.Engine.Contracts.Rendering;

    using SlimDX;

    public sealed class LightInstruction
    {
        public ILight Light { get; set; }

        public Vector4 Position { get; set; }

        public Matrix World { get; set; }
    }
}
