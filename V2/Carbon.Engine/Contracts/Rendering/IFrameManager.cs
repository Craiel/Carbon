using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface IFrameManager : IEngineComponent
    {
        bool EnableDebugOverlay { get; set; }

        void Resize(TypedVector2<int> size);

        FrameInstructionSet BeginSet(ICamera camera);

        void BeginFrame();
        void RenderSet(FrameInstructionSet set);

        void ClearCache();
    }
}
