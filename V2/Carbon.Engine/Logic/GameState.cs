using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;

namespace Carbon.Engine.Logic
{
    public abstract class GameState : EngineComponent, IGameState
    {

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScriptingEngine ScriptingEngine { get; protected set; }
        public ISceneManager SceneManager { get; protected set; }
        public IContentManager ContentManager { get; protected set; }
        public IResourceManager ResourceManager { get; protected set; }
    }
}
