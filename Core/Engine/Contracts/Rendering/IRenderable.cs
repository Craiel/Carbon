namespace Core.Engine.Contracts.Rendering
{
    using Core.Engine.Logic;
    using Core.Engine.Rendering;

    public interface IRenderableComponent
    {
        void Render(IFrameManager frameManager);
        void Resize(TypedVector2<int> size);
    }

    public interface IRenderable
    {
        void Render(FrameInstructionSet activeSet);
    }
}
