using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;

using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Rendering.RenderTarget
{
    class GBufferRenderTarget : RenderTargetBase
    {
        private readonly Texture2D[] textures;
        private readonly RenderTargetView[] targetViews;
        private readonly ShaderResourceView[] views;

        private Texture2D depthStencil;
        private DepthStencilView depthStencilView;

        private Texture2DDescription desiredTexture;
        private RenderTargetViewDescription desiredTargetView;

        private Texture2DDescription desiredDepthStencil;
        private DepthStencilViewDescription desiredDepthStencilView;
        
        private ShaderResourceView depthView;

        private bool isResizing;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GBufferRenderTarget()
        {
            this.textures = new Texture2D[3];
            this.targetViews = new RenderTargetView[3];
            this.views = new ShaderResourceView[3];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Dispose()
        {
            this.DisposeResources();

            base.Dispose();
        }

        public ShaderResourceView NormalView
        {
            get
            {
                return this.views[0];
            }
        }

        public ShaderResourceView DiffuseView
        {
            get
            {
                return this.views[1];
            }
        }

        public ShaderResourceView SpecularView
        {
            get
            {
                return this.views[2];
            }
        }

        public ShaderResourceView DepthView
        {
            get
            {
                return this.depthView;
            }
        }

        public override void Clear(ICarbonGraphics graphics, Vector4 color)
        {
            if (this.isResizing)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                graphics.ImmediateContext.ClearRenderTargetView(this.targetViews[i], new Color4(color));
            }

            graphics.ImmediateContext.ClearDepthStencilView(this.depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public override void Set(ICarbonGraphics graphics)
        {
            if (this.isResizing)
            {
                return;
            }

            // Set the target views and viewport
            graphics.ImmediateContext.OutputMerger.SetTargets(this.depthStencilView, this.targetViews);
            graphics.ImmediateContext.Rasterizer.SetViewports(this.Viewport);

            base.Set(graphics);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoResize(ICarbonGraphics graphics, TypedVector2<int> size)
        {
            this.isResizing = true;

            this.DisposeResources();

            this.desiredTexture = new Texture2DDescription
                {
                    Width = size.X,
                    Height = size.Y,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                };

            this.desiredTargetView = new RenderTargetViewDescription
                {
                    Dimension = RenderTargetViewDimension.Texture2D,
                    MipSlice = 0
                };

            // Recreate the Depth Stencil
            this.desiredDepthStencil = new Texture2DDescription
            {
                Width = size.X,
                Height = size.Y,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R24G8_Typeless,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };

            // Recreate the Depth Stencil View
            this.desiredDepthStencilView = new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
            };

            var desiredDepthView = new ShaderResourceViewDescription
            {
                Dimension = ShaderResourceViewDimension.Texture2D,
                MipLevels = this.desiredDepthStencil.MipLevels,
                MostDetailedMip = 0,
                Format = Format.R24_UNorm_X8_Typeless
            };
            
            // 2 Component signed normalized spheremap-encoded normals
            this.desiredTexture.Format = Format.R16G16B16A16_SNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            this.textures[0] = graphics.StateManager.GetTexture(this.desiredTexture);
            this.targetViews[0] = graphics.StateManager.GetRenderTargetView(this.textures[0], this.desiredTargetView);
            this.views[0] = new ShaderResourceView(graphics.ImmediateContext.Device, this.textures[0]);

            // 3 Component unsigned normalized diffuse albedo
            this.desiredTexture.Format = Format.R10G10B10A2_UNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            this.textures[1] = graphics.StateManager.GetTexture(this.desiredTexture);
            this.targetViews[1] = graphics.StateManager.GetRenderTargetView(this.textures[1], this.desiredTargetView);
            this.views[1] = new ShaderResourceView(graphics.ImmediateContext.Device, this.textures[1]);

            // 4 Component unsigned normalized specular albedo and power
            this.desiredTexture.Format = Format.R8G8B8A8_UNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            this.textures[2] = graphics.StateManager.GetTexture(this.desiredTexture);
            this.targetViews[2] = graphics.StateManager.GetRenderTargetView(this.textures[2], this.desiredTargetView);
            this.views[2] = new ShaderResourceView(graphics.ImmediateContext.Device, this.textures[2]);

            this.depthStencil = graphics.StateManager.GetTexture(this.desiredDepthStencil);
            this.depthStencilView = graphics.StateManager.GetDepthStencilView(this.desiredDepthStencilView, this.depthStencil);
            this.depthView = new ShaderResourceView(graphics.ImmediateContext.Device, this.depthStencil, desiredDepthView);
            
            this.isResizing = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DisposeResources()
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.views[i] != null)
                {
                    this.views[i].Dispose();
                    this.views[i] = null;
                }
                
                if (this.targetViews[i] != null)
                {
                    this.targetViews[i].Dispose();
                    this.targetViews[i] = null;
                }

                if (this.textures[i] != null)
                {
                    this.textures[i].Dispose();
                    this.textures[i] = null;
                }
            }

            if (this.depthView != null)
            {
                this.depthView.Dispose();
                this.depthView = null;
            }

            if (this.depthStencilView != null)
            {
                this.depthStencilView.Dispose();
                this.depthStencilView = null;
            }

            if (this.depthStencil != null)
            {
                this.depthStencil.Dispose();
                this.depthStencil = null;
            }
        }
    }
}
