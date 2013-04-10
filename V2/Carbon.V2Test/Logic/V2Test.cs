using System;
using System.Drawing;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using Carbon.V2Test.Contracts;
using Carbon.V2Test.Scenes;

using Core.Utils;
using Core.Utils.Contracts;

namespace Carbon.V2Test.Logic
{
    public enum SceneKeys
    {
        Test = 1,
        Test2 = 2
    }

    public class V2Test : CarbonGame, IV2Test
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;
        private readonly IV2TestGameState gameState;
        private readonly IRenderer renderer;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public V2Test(IEngineFactory factory)
            : base(factory)
        {
            this.factory = factory;
            this.log = factory.Get<IApplicationLog>().AquireContextLog("Carbon.V2Test");
            this.renderer = factory.Get<IRenderer>();

            this.gameState = factory.Get<IV2TestGameState>();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string InternalGameName
        {
            get { return "V2Test"; }
        }

        protected override void Initialize()
        {
            this.log.Info("Initializing V2Test");

            base.Initialize();

            this.gameState.Initialize(this.Graphics);

            this.gameState.ScriptingEngine.Register(new V2GameScriptingProvider(this));

            this.gameState.SceneManager.Register((int)SceneKeys.Test, factory.Get<ITestScene>());
            this.gameState.SceneManager.Register((int)SceneKeys.Test2, factory.Get<ITestScene2>());

            var content = new EngineContent { FallbackTexture = HashUtils.BuildResourceHash(@"Textures\default.dds") };
            this.SetEngineContent(content);

            // Set our initial size
            this.Window.Size = new Size(1024, 768);
           
            this.gameState.SceneManager.Activate((int)SceneKeys.Test);
            this.gameState.SceneManager.Resize(this.Window.Size.Width, this.Window.Size.Height);
        }

        protected override void OnWindowResize(object sender, EventArgs eventArgs)
        {
            lock (this.RenderSynchronizationLock)
            {
                base.OnWindowResize(sender, eventArgs);

                this.log.Debug("OnWindowResize {0}", this.Window.Location.ToString());

                // Todo: need to fix this
                //this.Cursor.MinPosition = new Vector2(this.Window.Location.X, this.Window.Location.Y);
                //this.Cursor.MaxPosition = new Vector2(this.Window.Location.X + this.Window.ClientSize.Width, this.Window.Location.Y + this.Window.ClientSize.Height);

                this.gameState.SceneManager.Resize(this.Window.ClientSize.Width, this.Window.ClientSize.Height);
            }
        }

        protected override void Update(ITimer gameTime)
        {
            base.Update(gameTime);
            
            this.gameState.Update(gameTime);

            // Some useful debug output next
            this.Window.Text = string.Format("V2Test GameTime: {0:hh\\:mm\\:ss\\:fff}, FPS: {1}", gameTime.ElapsedTime, this.FramesPerSecond);

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

            renderer.BeginFrame();
            frameManager.BeginFrame();

            this.gameState.SceneManager.Render(frameManager);

            renderer.EndFrame();
        }

        protected override void OnClose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            base.OnClose(sender, e);

            this.gameState.Dispose();
        }

        public void SwitchScene(SceneKeys key)
        {
            lock (this.RenderSynchronizationLock)
            {
                this.gameState.NodeManager.Clear();
                this.gameState.SceneManager.Activate((int)key);
                this.gameState.SceneManager.Resize(this.Window.Size.Width, this.Window.Size.Height);
            }
        }

        public void Reload()
        {
            lock (this.RenderSynchronizationLock)
            {
                this.Graphics.ClearCache();
                this.gameState.ResourceManager.ClearCache();
                this.gameState.ContentManager.ClearCache();
                this.renderer.ClearCache();

                this.gameState.NodeManager.Clear();
                this.gameState.SceneManager.Reload();
            }
        }
    }
}
