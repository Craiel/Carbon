namespace GrandSeal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using Contracts;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;
    using Core.Utils;
    using Core.Utils.Contracts;

    using Ninject;

    using SlimDX;

    public enum SceneKey
    {
        Entry = 0,
        MainMenu = 1,
    }

    public class GrandSeal : CarbonGame, IGrandSeal
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;
        private readonly IGrandSealGameState gameState;
        private readonly IGrandSealScriptingProvider scriptingProvider;

        private bool clearCacheOnNextPass;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GrandSeal(IEngineFactory factory)
            : base(factory)
        {
            this.factory = factory;
            this.log = factory.Get<IApplicationLog>().AquireContextLog("GrandSeal");

            this.gameState = factory.Get<IGrandSealGameState>();
            this.scriptingProvider = factory.GetScriptingProvider(this);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void SwitchScene(int key)
        {
            lock (this.RenderSynchronizationLock)
            {
                this.gameState.ScriptingEngine.Unregister(this.gameState.SceneManager.ActiveScene);
                this.gameState.SceneManager.Activate(key);
                this.gameState.SceneManager.Resize(new TypedVector2<int>(this.Window.Size.Width, this.Window.Size.Height));
                this.gameState.ScriptingEngine.Register(this.gameState.SceneManager.ActiveScene);
            }
        }

        public void Reload()
        {
            lock (this.RenderSynchronizationLock)
            {
                this.Graphics.ClearCache();
                this.gameState.ResourceManager.ClearCache();
                this.gameState.ContentManager.ClearCache();
                this.clearCacheOnNextPass = true;

                this.gameState.SceneManager.Reload();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        protected override string InternalGameName
        {
            get { return "GrandSeal"; }
        }

        protected override void Initialize()
        {
            this.log.Info("Initializing");

            base.Initialize();

            this.gameState.Initialize(this.Graphics);

            this.gameState.ScriptingEngine.Register(this.scriptingProvider);
            
            var content = new EngineContent { FallbackTexture = HashUtils.BuildResourceHash(@"Textures\default.dds") };
            this.SetEngineContent(content);

            // Set our initial size
            var size = new TypedVector2<int>(1024, 768);
            this.Window.Size = new Size(size.X, size.Y);

            // Setup the entry scene
            var entryScene = this.factory.Get<ISceneEntry>();
            entryScene.SceneScriptHash = HashUtils.BuildResourceHash(@"Scripts\Entry\Init.lua");
            this.gameState.SceneManager.Register((int)SceneKey.Entry, entryScene);
           
            // Activate the entry scene
            this.gameState.SceneManager.Activate((int)SceneKey.Entry);
        }

        protected override void OnWindowResize(object sender, EventArgs eventArgs)
        {
            lock (this.RenderSynchronizationLock)
            {
                base.OnWindowResize(sender, eventArgs);

                this.log.Debug("OnWindowResize {0}", this.Window.Location.ToString());

                // Todo: need to fix this
                // this.Cursor.MinPosition = new Vector2(this.Window.Location.X, this.Window.Location.Y);
                // this.Cursor.MaxPosition = new Vector2(this.Window.Location.X + this.Window.ClientSize.Width, this.Window.Location.Y + this.Window.ClientSize.Height);

                this.gameState.SceneManager.Resize(new TypedVector2<int>(this.Window.ClientSize.Width, this.Window.ClientSize.Height));
            }
        }

        protected override void Update(ITimer gameTime)
        {
            base.Update(gameTime);
            
            this.gameState.Update(gameTime);

            // Some useful debug output next
            this.Window.Text = string.Format("GrandSeal GameTime: {0:hh\\:mm\\:ss\\:fff}, FPS: {1}", gameTime.ElapsedTime, this.FramesPerSecond);

            // Lock the cursor to the screencenter after everyone is done with the updates
            // this.cursor.Position = this.Window.Center;
        }
        
        protected override void Render(IRenderer renderer, IFrameManager frameManager)
        {
            // Don't render if our window is not set up yet or not sized
            if (this.Graphics.WindowViewport.Width <= 0 || this.Graphics.WindowViewport.Height <= 0)
            {
                return;
            }

            if (this.clearCacheOnNextPass)
            {
                this.clearCacheOnNextPass = false;
                renderer.ClearCache();
                frameManager.ClearCache();
            }

            frameManager.BackgroundColor = new Vector4(0.6f, 0.7f, 0.8f, 1);
            renderer.BeginFrame();
            frameManager.BeginFrame();

            this.gameState.SceneManager.Render(frameManager);

            renderer.EndFrame();
        }

        protected override void OnClose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.gameState.Dispose();

            base.OnClose(sender, e);
        }
    }
}
