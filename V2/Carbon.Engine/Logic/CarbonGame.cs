using System;
using System.Threading;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;

using Core.Utils;
using Core.Utils.Contracts;
using Core.Utils.Diagnostics;

using SlimDX.Windows;

namespace Carbon.Engine.Logic
{
    public struct EngineContent
    {
        public string FallbackTexture;
     }

    /// <summary>
    /// Base class for a carbon based game
    /// 
    /// 
    /// Todo:
    /// - Cleanup           !!!!!
    /// - 2D Rendering
    /// - Cleanup
    /// - Fonts
    /// - Debug overlay
    /// - Cleanup
    /// 
    /// NEXT!
    /// - Change Shader to material, have renderer choose shader based on parameters
    /// - Sort models by material to avoid shader switching
    /// - Render debug overlay:
    ///   * axis arrows using cone and torus (cube?)
    ///   * Bounding Box spheres
    /// 
    /// - Fix Grid to be Quad texture instead of mesh
    /// 
    /// 
    /// Later:
    /// - Deferred Rendering: http://www.gamedev.net/topic/589695-directx11-and-deferred-rendering/
    /// </summary>
    public abstract class CarbonGame : ICarbonGame
    {
        private static readonly TimeSpan FrameTime = TimeSpan.FromSeconds(1);
        private static readonly long TargetFrameRate = 60;

        private readonly ITimer gameTimer;
        private readonly ICarbonGraphics graphics;
        private readonly ILog log;

        private readonly IResourceManager coreResourceManager;
        private readonly IInputManager inputManager;
        private readonly IFrameManager mainFrameManager;
        private readonly IRenderer mainRenderer;
        private readonly IScriptingEngine scriptingEngine;
        private readonly IDebugController debugController;

        private CarbonWindow window;

        private bool isResizing;
        private bool isClosing;
        private bool limitFps = false;

        private float possibleFramesPerSecond;
        private int currentFrameDrop;

        private Thread renderThread;
        
        private TimeSpan frameAccumulator;
        private TimeSpan fpsAccumulator;
        private int frameCount;
        private int frameDropCount;
        
        private object renderSynchronizationLock = new object();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CarbonGame(IEngineFactory factory)
        {
            this.coreResourceManager = factory.GetResourceManager("Data");
            this.graphics = factory.GetGraphics(this.coreResourceManager);
            this.debugController = factory.Get<IDebugController>();
            this.inputManager = factory.Get<IInputManager>();
            this.mainFrameManager = factory.Get<IFrameManager>();
            this.mainRenderer = factory.Get<IRenderer>();
            this.scriptingEngine = factory.Get<IScriptingEngine>();
            this.log = factory.Get<IEngineLog>().AquireContextLog("CarbonGame");

            this.gameTimer = new Core.Utils.Timer();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float FramesPerSecond { get; private set; }
        public float FramesSkipped { get; private set; }

        public virtual void Run()
        {
            this.BeginInitialize();
            this.Initialize();

            this.gameTimer.Reset();

            this.log.Debug("Bringing up Render Thread");
            this.renderThread = new Thread(this.MainRenderLoop);
            this.renderThread.Start();

            Console.WriteLine("\n");
            this.log.Debug("------------------------------------------------------");
            this.log.Debug("Main Loop");

            MessagePump.Run(this.window, this.MainLoop);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected CarbonWindow Window
        {
            get
            {
                return this.window;
            }
        }

        protected IResourceManager CoreResourceManager
        {
            get
            {
                return this.coreResourceManager;
            }
        }

        protected ICarbonGraphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }

        protected IScriptingEngine ScriptingEngine
        {
            get
            {
                return this.scriptingEngine;
            }
        }
        
        protected abstract string InternalGameName { get; }

        protected virtual void Initialize()
        {
            this.inputManager.Initialize(this.graphics);
            this.mainRenderer.Initialize(this.graphics);
            this.mainFrameManager.Initialize(this.graphics);

            this.debugController.IsActive = true;
        }
        
        protected virtual void Update(ITimer gameTime)
        {
            this.inputManager.Update(gameTime);

            Thread.Sleep(1);
        }
        
        protected virtual void Render(IRenderer renderer, IFrameManager frameManager)
        {
            // Don't render if our window is not set up yet or not sized
            if (this.graphics.WindowViewport.Width <= 0 || this.graphics.WindowViewport.Height <= 0)
            {
                return;
            }

            renderer.BeginFrame();
            frameManager.BeginFrame();

            // Nothing to do here

            renderer.EndFrame();
        }

        protected virtual void OnWindowResize(object sender, EventArgs eventArgs)
        {
            lock (this.renderSynchronizationLock)
            {
                this.graphics.Resize(this.window.ClientSize.Width, this.window.ClientSize.Height);
                this.mainFrameManager.Resize(this.window.ClientSize.Width, this.window.ClientSize.Height);
            }
        }

        protected void SetEngineContent(EngineContent content)
        {
            this.graphics.TextureManager.Fallback = this.graphics.TextureManager.Register(content.FallbackTexture);
        }

        protected void SetupDefaultProviders(IScriptingEngine engine)
        {
            // Todo: Register the default providers
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void MainLoop()
        {
            using (new ProfileRegion("MainLoop - Update"))
            {
                this.Update(this.gameTimer);
            }
        }

        private void MainRenderLoop()
        {
            while (!this.isClosing)
            {
                // Todo: needs some work
                    TimeSpan updateTime = this.gameTimer.Update();
                    this.frameAccumulator += updateTime;
                    this.fpsAccumulator += updateTime;
                    this.possibleFramesPerSecond++;

                    if (this.fpsAccumulator >= FrameTime)
                    {
                        if (this.possibleFramesPerSecond > TargetFrameRate)
                        {
                            this.currentFrameDrop = (int)(this.possibleFramesPerSecond / TargetFrameRate);
                        }
                        else
                        {
                            this.currentFrameDrop = 0;
                        }

                        this.FramesPerSecond = this.frameCount;
                        this.FramesSkipped = 0;
                        this.fpsAccumulator = TimeSpan.FromTicks(0);
                        this.frameCount = 0;
                        this.possibleFramesPerSecond = 0;
                    }

                    if (!this.limitFps || this.frameDropCount > this.currentFrameDrop)
                    {
                        this.frameCount++;
                        this.frameAccumulator = TimeSpan.FromTicks(0);
                        this.frameDropCount = 0;

                        lock (this.renderSynchronizationLock)
                        {
                            using (new ProfileRegion("MainLoop - Render"))
                            {
                                this.Render(this.mainRenderer, this.mainFrameManager);
                            }
                        }
                    }
                    else
                    {
                        this.frameDropCount++;
                        this.FramesSkipped++;

                        Thread.Sleep(1);
                    }
            }
        }

        private void BeginInitialize()
        {
            Formatter.SetGlobal("GameName", this.InternalGameName);

            Console.WriteLine("\n");
            this.log.Debug("------------------------------------------------------");
            this.log.Debug("Initializing {0}", this.InternalGameName);

            this.window = new CarbonWindow();

            this.graphics.TargetHandle = this.window.Handle;
            this.graphics.Reset();
            this.graphics.Resize(this.window.ClientSize.Width, this.window.ClientSize.Height);

            // After all is hooked up register the window events
            this.window.Resize += this.OnWindowResize;
            this.window.ResizeBegin += (sender, args) => { this.isResizing = true; };
            this.window.ResizeEnd += (sender, args) => { this.isResizing = false; this.OnWindowResize(sender, args); };
            this.window.FormClosing += this.OnClosing;
            this.window.FormClosed += this.OnClose;

            // Setup the scripting
            this.SetupDefaultProviders(this.scriptingEngine);
        }

        private void OnClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Console.WriteLine("\n");
            this.log.Debug("------------------------------------------------------");
            this.log.Debug("Closing Down");
            this.isClosing = true;
            while(this.renderThread.IsAlive)
            {
                Thread.Sleep(10);
            }
        }

        protected virtual void OnClose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.gameTimer.Pause();
            
            this.inputManager.Dispose();
            this.mainRenderer.Dispose();
            this.mainFrameManager.Dispose();
            this.graphics.Dispose();

#if DEBUG
            Profiler.TraceProfilerStatistics();
#endif
        }
    }
}
