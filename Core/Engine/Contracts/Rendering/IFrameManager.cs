namespace Core.Engine.Contracts.Rendering
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;

    using SharpDX;

    public interface IFrameManager : IEngineComponent
    {
        Vector4 BackgroundColor { get; set; }

        void Resize(TypedVector2<int> size);

        FrameInstructionSet BeginSet(ICamera camera);

        void BeginFrame();
        void RenderSet(FrameInstructionSet set);

        void ClearCache();
    }
}
