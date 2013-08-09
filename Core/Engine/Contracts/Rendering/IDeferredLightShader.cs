namespace Core.Engine.Contracts.Rendering
{
    using SlimDX;

    public interface IDeferredLightShader : ICarbonShader
    {
        void SetAmbient(Vector4 color);
        void SetDirectional(Vector4 position, Vector3 direction, Vector4 color);
        void SetPoint(Vector4 position, Vector4 color, float range, Matrix lightViewProjection);
        void SetSpot(Vector4 position, Vector3 direction, Vector4 color, float range, Vector2 angles, bool useShadowMapping, Matrix lightViewProjection);
    }
}
