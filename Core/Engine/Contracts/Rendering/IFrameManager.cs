using Core.Engine.Contracts.Logic;
using Core.Engine.Logic;
using Core.Engine.Rendering;

namespace Core.Engine.Contracts.Rendering
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
