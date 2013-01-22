using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface IFrameManager : IEngineComponent
    {
        bool EnableDebugOverlay { get; set; }

        void Resize(int width, int height);

        FrameInstructionSet BeginSet(ICamera camera);

        void BeginFrame();
        void RenderSet(FrameInstructionSet set);
    }
}
