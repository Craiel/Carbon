namespace Core.Engine.Contracts.Rendering
{
    using SlimDX;

    public interface IDefaultShader : ICarbonShader
    {
        Vector4 AmbientLight { get; set; }

        void ClearLight();
        void AddDirectionalLight(Vector3 direction, Vector4 color, float specularPower = 1.0f);
        void AddPointLight(Vector4 position, Vector4 color, float range, float specularPower = 1.0f);
        void AddSpotLight(Vector4 position, Vector3 direction, Vector4 color, float range, Vector2 angles, float specularPower = 1.0f);
    }
}
