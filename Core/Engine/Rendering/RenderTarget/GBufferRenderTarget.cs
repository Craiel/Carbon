namespace Core.Engine.Rendering.RenderTarget
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;

    internal class GBufferRenderTarget : RenderTargetBase
    {
        private readonly TextureData[] textures;
        private readonly RenderTargetView[] targetViews;

        private TextureData depthStencil;
        private DepthStencilView depthStencilView;

        private Texture2DDescription desiredTexture;
        private RenderTargetViewDescription desiredTargetView;

        private Texture2DDescription desiredDepthStencil;
        private DepthStencilViewDescription desiredDepthStencilView;
        
        private bool isResizing;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GBufferRenderTarget()
        {
            this.textures = new TextureData[3];
            this.targetViews = new RenderTargetView[3];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TextureData NormalData
        {
            get
            {
                return this.textures[0];
            }
        }

        public TextureData DiffuseData
        {
            get
            {
                return this.textures[1];
            }
        }

        public TextureData SpecularData
        {
            get
            {
                return this.textures[2];
            }
        }

        public TextureData DepthData
        {
            get
            {
                return this.depthStencil;
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
            graphics.ImmediateContext.Rasterizer.SetViewport(this.Viewport);

            base.Set(graphics);
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

            this.DisposeResources();

            base.Dispose(true);
        }

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
                    Texture2D = new RenderTargetViewDescription.Texture2DResource { MipSlice = 0 }
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
                Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
            };

            var desiredDepthView = new ShaderResourceViewDescription
            {
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                                {
                                    MipLevels = this.desiredDepthStencil.MipLevels,
                                    MostDetailedMip = 0
                                },
                Format = Format.R24_UNorm_X8_Typeless
            };
            
            // 2 Component signed normalized spheremap-encoded normals
            this.desiredTexture.Format = Format.R16G16B16A16_SNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            Texture2D texture = graphics.StateManager.GetTexture(this.desiredTexture);
            var view = new ShaderResourceView(graphics.ImmediateContext.Device, texture);
            this.targetViews[0] = graphics.StateManager.GetRenderTargetView(texture, this.desiredTargetView);
            this.textures[0] = new TextureData(texture, view);

            // 3 Component unsigned normalized diffuse albedo
            this.desiredTexture.Format = Format.R10G10B10A2_UNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            texture = graphics.StateManager.GetTexture(this.desiredTexture);
            view = new ShaderResourceView(graphics.ImmediateContext.Device, texture);
            this.targetViews[1] = graphics.StateManager.GetRenderTargetView(texture, this.desiredTargetView);
            this.textures[1] = new TextureData(texture, view);

            // 4 Component unsigned normalized specular albedo and power
            this.desiredTexture.Format = Format.R8G8B8A8_UNorm;
            this.desiredTargetView.Format = this.desiredTexture.Format;
            texture = graphics.StateManager.GetTexture(this.desiredTexture);
            view = new ShaderResourceView(graphics.ImmediateContext.Device, texture);
            this.targetViews[2] = graphics.StateManager.GetRenderTargetView(texture, this.desiredTargetView);
            this.textures[2] = new TextureData(texture, view);

            texture = graphics.StateManager.GetTexture(this.desiredDepthStencil);
            view = new ShaderResourceView(graphics.ImmediateContext.Device, texture, desiredDepthView);
            this.depthStencilView = graphics.StateManager.GetDepthStencilView(this.desiredDepthStencilView, texture);
            this.depthStencil = new TextureData(texture, view);
            
            this.isResizing = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DisposeResources()
        {
            for (int i = 0; i < 3; i++)
            {
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
