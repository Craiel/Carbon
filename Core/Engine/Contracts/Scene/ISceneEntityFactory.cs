namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Resource.Resources.Stage;

    public interface ISceneEntityFactory : IEngineComponent
    {
        ICameraEntity BuildCamera(StageCameraElement cameraElement);
        ILightEntity BuildLight(StageLightElement lightElement);
    }
}
