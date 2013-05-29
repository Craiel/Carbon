using System.Collections.Generic;

using Core.Engine.Contracts.Logic;
using Core.Engine.Rendering;

namespace Core.Engine.Contracts.Rendering
{
    public interface IRenderer : IEngineComponent
    {
        IList<FrameStatistics> FrameStatistics { get; }

        void BeginFrame();

        void AddForwardLighting(RenderLightInstruction instruction);
        void AddForwardLighting(IList<RenderLightInstruction> instructions);

        void SetDeferredLighting(RenderLightInstruction instruction);

        void Render(RenderParameters parameters, RenderInstruction instructions);

        void EndFrame();

        void ClearCache();
    }
}
