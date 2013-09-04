namespace Core.Engine.Rendering
{
    using System;

    using SharpDX;

    public sealed class RenderLightInstruction
    {
        public bool IsCastingShadow { get; set; }
        public bool RegenerateShadowMap { get; set; }

        public LightType Type { get; set; }

        public Vector3 Position { get; set; }
        public Vector4 Color { get; set; }

        public Vector3 Direction { get; set; }
        public Vector2 SpotAngles { get; set; }

        public float Range { get; set; }
        public float SpecularPower { get; set; }

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Mesh Mesh { get; set; }

        public int ShadowMapSize { get; set; }

        public int GetShadowMapKey()
        {
            return Tuple.Create(this.Position, this.Range, this.View, this.Projection).GetHashCode();
        }
    }
}
