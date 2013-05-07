using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic.Scripting;

namespace Carbon.Engine.Logic
{
    public abstract class GameState : EngineComponent, IGameState
    {
        protected GameState()
        {
        }

        protected GameState(IEngineFactory factory)
        {
            this.SceneManager = factory.Get<ISceneManager>();

            this.ScriptingEngine = factory.Get<IScriptingEngine>();
            this.ScriptingEngine.Register(new ScriptingCoreProvider(factory.Get<IEngineLog>()));
            this.ScriptingEngine.Register(factory.Get<IInputManager>());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScriptingEngine ScriptingEngine { get; protected set; }
        public ISceneManager SceneManager { get; protected set; }
        public IContentManager ContentManager { get; protected set; }
        public IResourceManager ResourceManager { get; protected set; }
        public ISceneEntityFactory SceneEntityFactory { get; protected set; }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            if (this.SceneManager != null)
            {
                this.SceneManager.Initialize(graphics);
            }

            if (this.SceneEntityFactory != null)
            {
                this.SceneEntityFactory.Initialize(graphics);
            }
        }

        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.SceneManager != null)
            {
                this.SceneManager.Update(gameTime);
            }

            if (this.SceneEntityFactory != null)
            {
                this.SceneEntityFactory.Update(gameTime);
            }
            
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.SceneManager != null)
            {
                this.SceneManager.Dispose();
            }

            if (this.SceneEntityFactory != null)
            {
                this.SceneEntityFactory.Dispose();
            }

            if (this.ContentManager != null)
            {
                this.ContentManager.Dispose();
            }

            if (this.ResourceManager != null)
            {
                this.ResourceManager.Dispose();
            }
        }
    }
}
