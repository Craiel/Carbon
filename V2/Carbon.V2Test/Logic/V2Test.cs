using System;
using System.Drawing;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Logic;
using Carbon.Engine.Resource;
using Carbon.V2Test.Contracts;
using Carbon.V2Test.Scenes;
using Core.Utils.Contracts;
using SlimDX;

namespace Carbon.V2Test.Logic
{
    public class V2Test : CarbonGame, IV2Test
    {
        private readonly ILog log;

        private readonly ITestScene testScene;
        private readonly ITestSceneSponza testSceneSponza;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public V2Test(IEngineFactory factory)
            : base(factory)
        {                        
            this.testScene = factory.Get<ITestScene>();
            this.testSceneSponza = factory.Get<ITestSceneSponza>();

            this.log = factory.Get<IApplicationLog>().AquireContextLog("Carbon.V2Test");

            factory.Get<IResourceManager>().AddContent(new FolderContent(@"..\..\..\Data\", useSources:true));
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected override string InternalGameName
        {
            get { return "V2Test"; }
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.testScene.Initialize(this.Graphics);
            //this.testSceneSponza.Initialize(this.Graphics);

            // Set our initial size
            this.Window.Size = new Size(1024, 768);

            // Register some default data into the engine
            var content = new EngineContent { FallbackTexture = @"Textures\checkerboard.dds" };
            this.SetEngineContent(content);

            this.log.Info("Initializing V2Test");
        }

        protected override void OnWindowResize(object sender, EventArgs eventArgs)
        {
            base.OnWindowResize(sender, eventArgs);

            this.log.Debug("OnWindowResize {0}", this.Window.Location.ToString());

            this.Cursor.MinPosition = new Vector2(this.Window.Location.X, this.Window.Location.Y);
            this.Cursor.MaxPosition = new Vector2(this.Window.Location.X + this.Window.ClientSize.Width, this.Window.Location.Y + this.Window.ClientSize.Height);

            this.testScene.Resize(this.Window.ClientSize.Width, this.Window.ClientSize.Height);
            this.testSceneSponza.Resize(this.Window.ClientSize.Width, this.Window.ClientSize.Height);
        }

        protected override void Update(ITimer gameTime)
        {
            base.Update(gameTime);
            
            // Scenes next
            this.testScene.Update(gameTime);
            //this.testSceneSponza.Update(gameTime);

            // Some useful debug output next
            this.Window.Text = string.Format("V2Test GameTime: {0:hh\\:mm\\:ss\\:fff}, FPS: {1}, Cursor: {2}", gameTime.ElapsedTime, this.FramesPerSecond, this.Cursor.Position);

            // Lock the cursor to the screencenter after everyone is done with the updates
            //this.cursor.Position = this.Window.Center;
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

            this.testScene.Render();
            //this.testSceneSponza.Render();

            renderer.EndFrame();
        }

        protected override void OnClose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            base.OnClose(sender, e);

            this.testScene.Dispose();
            this.testSceneSponza.Dispose();
        }
    }
}
