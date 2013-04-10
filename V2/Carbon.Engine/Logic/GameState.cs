using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;

namespace Carbon.Engine.Logic
{
    using Carbon.Engine.Contracts;
    using Carbon.Engine.Logic.Scripting;
    using Carbon.Engine.Scene;

    public abstract class GameState : EngineComponent, IGameState
    {
        protected GameState()
        {
        }

        protected GameState(IEngineFactory factory)
        {
            this.NodeManager = factory.Get<INodeManager>();

            this.SceneManager = factory.Get<ISceneManager>();

            this.ScriptingEngine = factory.Get<IScriptingEngine>();
            this.ScriptingEngine.Register(new ScriptingCoreProvider(factory.Get<IEngineLog>()));
            this.ScriptingEngine.Register(factory.Get<IInputManager>());
            this.ScriptingEngine.Register(this.NodeManager);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScriptingEngine ScriptingEngine { get; protected set; }
        public ISceneManager SceneManager { get; protected set; }
        public INodeManager NodeManager { get; protected set; }
        public IContentManager ContentManager { get; protected set; }
        public IResourceManager ResourceManager { get; protected set; }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            if (this.SceneManager != null)
            {
                this.SceneManager.Initialize(graphics);
            }

            if (this.NodeManager != null)
            {
                this.NodeManager.Initialize(graphics);
            }
        }

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            if (this.SceneManager != null)
            {
                this.SceneManager.Update(gameTime);
            }

            if (this.NodeManager != null)
            {
                this.NodeManager.Update(gameTime);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.SceneManager != null)
            {
                this.SceneManager.Dispose();
            }

            if (this.NodeManager != null)
            {
                this.NodeManager.Dispose();
            }
        }
    }
}
