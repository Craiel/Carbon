namespace Core.Engine.Rendering.RenderTarget
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    using SlimDX;
    using SlimDX.Direct3D11;
    using SlimDX.DXGI;

    internal class TextureRenderTarget : RenderTargetBase
    {
        private TextureData texture;
        private RenderTargetView targetView;

        private Texture2D depthStencil;
        private DepthStencilView depthStencilView;

        private BlendState blendState;

        private Texture2DDescription desiredTexture;
        private RenderTargetViewDescription desiredTargetView;

        private Texture2DDescription desiredDepthStencil;
        private DepthStencilViewDescription desiredDepthStencilView;
        
        private bool isResizing;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TextureData Data
        {
            get
            {
                return this.texture;
            }
        }

        public override void Dispose()
        {
            this.DisposeResources();

            base.Dispose();
        }
        
        public override void Clear(ICarbonGraphics graphics, Vector4 color)
        {
            if (this.isResizing)
            {
                return;
            }

            graphics.ImmediateContext.ClearRenderTargetView(this.targetView, new Color4(color));
            graphics.ImmediateContext.ClearDepthStencilView(this.depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public override void Set(ICarbonGraphics graphics)
        {
            if (this.isResizing)
            {
                return;
            }

            // Set the target views and viewport
            graphics.ImmediateContext.OutputMerger.SetTargets(this.depthStencilView, this.targetView);
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
                    Format = Format.R32G32B32A32_Float,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                };

            this.desiredTargetView = new RenderTargetViewDescription
                {
                    Format = this.desiredTexture.Format,
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
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };

            // Recreate the Depth Stencil View
            this.desiredDepthStencilView = new DepthStencilViewDescription
            {
                Format = this.desiredDepthStencil.Format,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
            };

            Texture2D newTexture = graphics.StateManager.GetTexture(this.desiredTexture);
            var view = new ShaderResourceView(graphics.ImmediateContext.Device, newTexture);
            this.targetView = graphics.StateManager.GetRenderTargetView(newTexture, this.desiredTargetView);
            this.texture = new TextureData(newTexture, view);

            this.depthStencil = graphics.StateManager.GetTexture(this.desiredDepthStencil);
            this.depthStencilView = graphics.StateManager.GetDepthStencilView(this.desiredDepthStencilView, this.depthStencil);
            
            this.isResizing = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DisposeResources()
        {
            if (this.blendState != null)
            {
                this.blendState.Dispose();
                this.blendState = null;
            }
            
            if (this.targetView != null)
            {
                this.targetView.Dispose();
                this.targetView = null;
            }

            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
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
