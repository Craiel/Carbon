namespace Core.Engine.Contracts.Logic
{
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Contracts.Scene;

    public interface IGameState : IEngineComponent
    {
        IScriptingEngine ScriptingEngine { get; }
        ISceneManager SceneManager { get; }
        IContentManager ContentManager { get; }
        IResourceManager ResourceManager { get; }
    }
}
