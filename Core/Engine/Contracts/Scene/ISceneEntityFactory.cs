namespace Core.Engine.Contracts.Scene
{
    using CarbonCore.Processing.Resource.Stage;

    using Core.Engine.Contracts.Logic;

    public interface ISceneEntityFactory : IEngineComponent
    {
        ICameraEntity BuildCamera(StageCameraElement cameraElement);
        ILightEntity BuildLight(StageLightElement lightElement);
    }
}
