using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Contracts.Scene
{
    using Carbon.Engine.Resource.Resources.Model;

    public interface ISceneEntityFactory : IScriptingProvider, IEngineComponent
    {
        void RotateEntity(ISceneEntity entity, Vector3 axis, float angle);

        ISceneEntity AddAmbientLight(Vector4 color, float specularPower = 1.0f);
        ISceneEntity AddDirectionalLight(Vector4 color, Vector3 direction, float specularPower = 1.0f);
        ISceneEntity AddPointLight(Vector4 color, float range = 1.0f, float specularPower = 1.0f);
        ISceneEntity AddSpotLight(Vector4 color, Vector2 angles, Vector3 direction, float range = 1.0f, float specularPower = 1.0f);

        ISceneEntity AddModel(ModelResource resource);
        ISceneEntity AddModel(string path);
        ISceneEntity AddSphere(int detailLevel);
        ISceneEntity AddPlane();
        ISceneEntity AddStaticText(int fontId, string text, Vector2 charSize);
    }
}
