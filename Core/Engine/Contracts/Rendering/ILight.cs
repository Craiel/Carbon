namespace Core.Engine.Contracts.Rendering
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Rendering;

    using SharpDX;

    public interface ILight : IEngineComponent
    {
        bool IsCastingShadow { get; set; }
        bool NeedShadowUpdate { get; set; }

        LightType Type { get; set; }

        Vector4 Color { get; set; }
        Vector3 Position { get; set; }

        Vector3 Direction { get; set; }

        Vector2 SpotAngles { get; set; }

        float Range { get; set; }

        float SpecularPower { get; set; }

        Matrix View { get; }
        Matrix Projection { get; }
    }
}
