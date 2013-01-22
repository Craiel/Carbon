using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface IRenderable
    {
        void Render(FrameInstructionSet activeSet);
    }
}
