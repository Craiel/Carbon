using System.Collections.Generic;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Contracts.Rendering
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
    }
}
