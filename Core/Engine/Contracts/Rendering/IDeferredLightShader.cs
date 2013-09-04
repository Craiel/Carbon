namespace Core.Engine.Contracts.Rendering
{
    using SharpDX;

    public interface IDeferredLightShader : ICarbonShader
    {
        void SetAmbient(Vector4 color);
        void SetDirectional(Vector3 position, Vector3 direction, Vector4 color);
        void SetPoint(Vector3 position, Vector4 color, float range, Matrix lightViewProjection);
        void SetSpot(Vector3 position, Vector3 direction, Vector4 color, float range, Vector2 angles, bool useShadowMapping, Matrix lightViewProjection);
    }
}
