namespace Core.Engine.Logic
{
    using System;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    using Contracts.Logic;
    using Contracts.Resource;
    using Rendering;
    using SharpDX;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    
    public class CarbonGraphics : ICarbonGraphics
    {
        private IResourceManager resourceManager;

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
        public CarbonGraphics(IFactory factory, IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;

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

        public CullMode CullMode
        {
            get
            {
                return this.desiredRasterizerState.CullMode;
            }

            set
            {
                if (this.desiredRasterizerState.CullMode != value)
                {
                    this.desiredRasterizerState.CullMode = value;
                    this.needRasterizerStateUpdate = true;
                }
            }
        }

        public bool IsDepthEnabled
        {
            get
            {
                return this.desiredDepthStencilState.IsDepthEnabled;
            }

            set
            {
                if (this.desiredDepthStencilState.IsDepthEnabled != value)
                {
                    this.desiredDepthStencilState.IsDepthEnabled = value;
                    this.needDepthStateUpdate = true;
                }
            }
        }

        public FillMode FillMode
        {
            get
            {
                return this.desiredRasterizerState.FillMode;
            }

            set
            {
                if (this.desiredRasterizerState.FillMode != value)
                {
                    this.desiredRasterizerState.FillMode = value;
                    this.needRasterizerStateUpdate = true;
                }
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

        public void SetResources(IResourceManager resources)
        {
            this.resourceManager = resources;
        }

        public void Reset()
        {
            this.settings = new DeviceSettings { CreationFlags = DeviceCreationFlags.None, ScreenSize = new TypedVector2<int>(640, 480) };
        }

        public void Resize(TypedVector2<int> size)
        {
            if (this.isResizing || (this.settings.ScreenSize == size))
            {
                return;
            }

            this.EnsureContext();

            this.isResizing = true;
            lock (this.context)
            {
                System.Diagnostics.Trace.TraceInformation("Resizing to {0}x{1}", size.X, size.Y);

                this.windowViewport = new Viewport(0, 0, size.X, size.Y);

                this.settings.ScreenSize = size;

                this.DisposeBuffers();

                this.context.SwapChain.ResizeBuffers(
                    1,
                    size.X,
                    size.Y,
                    this.context.SwapChain.Description.ModeDescription.Format,
                    this.context.SwapChain.Description.Flags);

                this.backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(this.context.SwapChain, 0);
                this.backBufferView = new RenderTargetView(this.context.Device, this.backBuffer);
            }

            this.isResizing = false;
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

            if (this.shaderManager != null)
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

            System.Diagnostics.Trace.TraceInformation("Acquiring DeviceContext");
            if (this.TargetHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Context creation requested before target point was set!");
            }

            this.context = new CarbonDeviceContextDx11(this.TargetHandle, this.settings);

            this.deviceStateManager = new DeviceStateManager(this.context.Device);

            this.shaderManager = new ShaderManager(this.resourceManager, this.context.Device);
            this.textureManager = new TextureManager(this.resourceManager);
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
