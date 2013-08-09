namespace Core.Engine.Contracts.Rendering
{
    using System;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Rendering;

    using SlimDX.Direct3D11;

    public interface ICarbonShader : IEngineComponent
    {
        bool LightingEnabled { get; set; }

        bool ForceReloadOnNextPass { get; set; }

        void Permutate();

        void Apply(DeviceContext context, Type vertexType, RenderParameters parameters, RenderInstruction instruction);

        void ResetConfigurationState(DeviceContext context);
    }
}
