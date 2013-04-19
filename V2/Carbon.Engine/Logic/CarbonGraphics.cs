using System;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Rendering;

using SlimDX.DXGI;
using SlimDX.Direct3D11;
using Core.Utils.Contracts;

namespace Carbon.Engine.Logic
{
    public class CarbonGraphics : ICarbonGraphics
    {
        private readonly ILog log;
        private readonly IEngineFactory factory;
        private readonly IResourceManager resourceManager;

        private CarbonDeviceContextDx11 context;
        private DeviceStateManager deviceStateManager;
        private ShaderManager shaderManager;
        private TextureManager textureManager;

        private DeviceSettings settings;

        private Texture2D backBuffer;
        private RenderTargetView backBufferView;

        private DepthStencilStateDescription desiredDepthStencilState;
        private RasterizerStateDescription desiredRasterizerState;

        private bool isResizing;

        private bool needRasterizerStateUpdate = true;
        private bool needDepthStateUpdate = true;

        private Viewport windowViewport;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonGraphics(IEngineFactory factory, IResourceManager resourceManager)
        {
            this.log = factory.Get<IEngineLog>().AquireContextLog("CarbonGraphics");
            this.resourceManager = resourceManager;
            this.factory = factory;

            this.desiredDepthStencilState = DeviceStateManager.DefaultDepthStencilState;
            this.desiredRasterizerState = DeviceStateManager.SolidRasterState;
            
            this.Reset();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IntPtr TargetHandle { get; set; }
        
        public DeviceContext ImmediateContext
        {
            get
            {
                this.EnsureContext();

                return this.context.Device.ImmediateContext;
            }
        }

        public RenderTargetView BackBufferView
        {
            get
            {
                this.EnsureContext();

                return this.backBufferView;
            }
        }

        public DeviceCreationFlags CreationFlags
        {
            get
            {
                return this.settings.CreationFlags;
            }

            set
            {
                this.settings.CreationFlags = value;
                this.ReleaseContext();
            }
        }

        public DeviceStateManager StateManager
        {
            get
            {
                this.EnsureContext();

                return this.deviceStateManager;
            }
        }

        public ShaderManager ShaderManager
        {
            get
            {
                this.EnsureContext();

                return this.shaderManager;
            }
        }

        public TextureManager TextureManager
        {
            get
            {
                this.EnsureContext();

                return this.textureManager;
            }
        }

        public Viewport WindowViewport
        {
            get
            {
                return this.windowViewport;
            }
        }

        public void Dispose()
        {
            this.DisposeBuffers();
            this.ReleaseContext();
        }

        public void Reset()
        {
            this.settings = new DeviceSettings { CreationFlags = DeviceCreationFlags.None, Width = 640, Height = 480 };
        }

        public void Resize(int width, int height)
        {
            if (this.isResizing || (this.settings.Width == width && this.settings.Height == height))
            {
                return;
            }

            this.EnsureContext();

            this.isResizing = true;
            lock (this.context)
            {
                this.log.Info("Resizing to {0}x{1}", width, height);

                this.windowViewport = new Viewport(0, 0, width, height);

                this.settings.Width = width;
                this.settings.Height = height;

                this.DisposeBuffers();

                this.context.SwapChain.ResizeBuffers(
                    1,
                    width,
                    height,
                    this.context.SwapChain.Description.ModeDescription.Format,
                    this.context.SwapChain.Description.Flags);

                this.backBuffer = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(this.context.SwapChain, 0);
                this.backBufferView = new RenderTargetView(this.context.Device, this.backBuffer);
            }

            this.isResizing = false;
        }

        public void SetCulling(CullMode mode)
        {
            this.desiredRasterizerState.CullMode = mode;
            this.needRasterizerStateUpdate = true;
        }
        
        public void EnableWireframe()
        {
            this.desiredRasterizerState = DeviceStateManager.WireFrameRasterDescription;
            this.needRasterizerStateUpdate = true;
        }

        public void DisableWireframe()
        {
            this.desiredRasterizerState = DeviceStateManager.SolidRasterState;
            this.needRasterizerStateUpdate = true;
        }

        public void EnableDepth()
        {
            this.desiredDepthStencilState.IsDepthEnabled = true;
            this.needDepthStateUpdate = true;
        }

        public void DisableDepth()
        {
            this.desiredDepthStencilState.IsDepthEnabled = false;
            this.needDepthStateUpdate = true;
        }

        public void ResetDepthState()
        {
            this.desiredDepthStencilState = DeviceStateManager.DefaultDepthStencilState;
            this.needDepthStateUpdate = true;
        }

        public void UpdateStates()
        {
            if (this.needDepthStateUpdate)
            {
                this.context.Device.ImmediateContext.OutputMerger.DepthStencilState = this.deviceStateManager.GetDepthStencilState(this.desiredDepthStencilState);
                this.needDepthStateUpdate = false;
            }

            if (this.needRasterizerStateUpdate)
            {
                this.context.Device.ImmediateContext.Rasterizer.State = this.deviceStateManager.GetRasterizerState(this.desiredRasterizerState);
                this.needRasterizerStateUpdate = false;
            }
        }

        public void Present(PresentFlags flags)
        {
            this.context.Present(flags);
        }

        public void ClearCache()
        {
            this.textureManager.ClearCache();
            this.shaderManager.ClearCache();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------    
        private void ReleaseContext()
        {
            if (this.deviceStateManager != null)
            {
                this.deviceStateManager.Dispose();
                this.deviceStateManager = null;
            }

            if(this.shaderManager != null)
            {
                this.shaderManager.Dispose();
                this.shaderManager = null;
            }

            if (this.textureManager != null)
            {
                this.textureManager.Dispose();
                this.textureManager = null;
            }

            if (this.context != null)
            {
                this.context.Dispose();
                this.context = null;
            }
        }

        private void EnsureContext()
        {
            if (this.context != null)
            {
                return;
            }

            this.log.Info("Acquiring DeviceContext");
            if (this.TargetHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Context creation requested before target point was set!");
            }

            this.context = new CarbonDeviceContextDx11(this.TargetHandle, this.settings);

            this.deviceStateManager = new DeviceStateManager(this.context.Device);

            this.shaderManager = new ShaderManager(this.resourceManager, this.context.Device);
            this.textureManager = new TextureManager(this.resourceManager, this.context.Device);
        }

        private void DisposeBuffers()
        {
            if (this.backBufferView != null)
            {
                this.backBufferView.Dispose();
                this.backBufferView = null;
            }

            if (this.backBuffer != null)
            {
                this.backBuffer.Dispose();
                this.backBuffer = null;
            }
        }
    }
}
