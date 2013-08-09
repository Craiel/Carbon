namespace Core.Engine.Contracts.Rendering
{
    using Core.Engine.Rendering.Shaders;

    public interface IDebugShader : ICarbonShader
    {
        DebugShaderMode Mode { get; set; }
    }
}
