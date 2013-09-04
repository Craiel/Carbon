namespace Core.Engine.Rendering
{
    using Core.Engine.Contracts.Rendering;

    using SharpDX;

    public sealed class LightInstruction
    {
        public ILight Light { get; set; }

        public Vector3 Position { get; set; }

        public Matrix World { get; set; }
    }
}
