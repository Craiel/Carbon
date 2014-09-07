namespace Core.Engine.Logic
{
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic.Scripting;

    public abstract class GameState : EngineComponent, IGameState
    {
        protected GameState()
        {
        }

        protected GameState(IFactory factory)
        {
            this.SceneManager = factory.Resolve<ISceneManager>();

            this.ScriptingEngine = factory.Resolve<IScriptingEngine>();
            this.ScriptingEngine.Register(new ScriptingCoreProvider());
            this.ScriptingEngine.Register(factory.Resolve<IInputManager>());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScriptingEngine ScriptingEngine { get; protected set; }
        public ISceneManager SceneManager { get; protected set; }
        public IContentManager ContentManager { get; protected set; }
        public IResourceManager ResourceManager { get; protected set; }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            if (this.SceneManager != null)
            {
                this.SceneManager.Initialize(graphics);
            }
        }

        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.SceneManager != null)
            {
                this.SceneManager.Update(gameTime);
            }
            
            return true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            base.Dispose(true);

            if (this.SceneManager != null)
            {
                this.SceneManager.Dispose();
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
