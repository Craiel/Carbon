namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Resource.Resources.Stage;
    using Core.Engine.Scene;

    public interface ISceneEntityFactory : IEngineComponent
    {
        ICamera BuildCamera(StageCameraElement cameraElement);
        ILight BuildLight(StageLightElement lightElement);
        IModelEntity BuildModel(StageModelElement modelElement);
    }
}
