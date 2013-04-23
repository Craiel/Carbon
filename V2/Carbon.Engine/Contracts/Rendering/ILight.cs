using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Rendering;

using SlimDX;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface ILight : IEngineComponent
    {
        LightType Type { get; set; }

        Vector4 Color { get; set; }
        Vector4 Position { get; set; }

        Vector3 Direction { get; set; }

        Vector2 SpotAngles { get; set; }

        float Range { get; set; }

        float SpecularPower { get; set; }

        Matrix View { get; }
        Matrix Projection { get; }
    }
}
