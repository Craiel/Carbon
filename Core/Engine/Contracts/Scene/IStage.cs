namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Scene;

    public interface IStage : IEngineComponent
    {
        SceneGraph BuildGraph();
    }
}
