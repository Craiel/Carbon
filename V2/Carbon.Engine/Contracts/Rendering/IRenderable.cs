using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface IRenderableComponent
    {
        void Render(IFrameManager frameManager);
        void Resize(int width, int height);
    }

    public interface IRenderable
    {
        void Render(FrameInstructionSet activeSet);
    }
}
