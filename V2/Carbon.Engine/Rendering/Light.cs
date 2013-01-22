using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public enum LightType
    {
        Ambient,
        Direction,
        Point,
        Spot
    }

    public class Light : EngineComponent, ILight
    {
        public Light()
        {
            this.Type = LightType.Ambient;
            this.SpecularPower = 1.0f;
        }

        public LightType Type { get; set; }

        public Vector4 Color { get; set; }

        public Vector3 Direction { get; set; }

        public Vector2 SpotAngles { get; set; }

        public float Range { get; set; }

        public float SpecularPower { get; set; }
    }
}
