using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IGameState : IEngineComponent
    {
        IScriptingEngine ScriptingEngine { get; }
        ISceneManager SceneManager { get; }
        IContentManager ContentManager { get; }
        IResourceManager ResourceManager { get; }
    }
}
