using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
{
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
