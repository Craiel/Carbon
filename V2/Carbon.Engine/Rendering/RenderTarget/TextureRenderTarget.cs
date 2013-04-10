using Carbon.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Rendering.RenderTarget
{
    internal class TextureRenderTarget : RenderTargetBase
    {
        private Texture2D texture;
        private RenderTargetView targetView;
        private ShaderResourceView textureView;

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

        public ShaderResourceView View
        {
            get
            {
                return this.textureView;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoResize(ICarbonGraphics graphics, int width, int height)
        {
            this.isResizing = true;

            this.DisposeResources();

            this.desiredTexture = new Texture2DDescription
                {
                    Width = width,
                    Height = height,
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
                Width = width,
                Height = height,
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

            this.texture = graphics.StateManager.GetTexture(this.desiredTexture);
            this.targetView = graphics.StateManager.GetRenderTargetView(this.texture, this.desiredTargetView);
            this.textureView = new ShaderResourceView(graphics.ImmediateContext.Device, this.texture);

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

            if (this.textureView != null)
            {
                this.textureView.Dispose();
                this.textureView = null;
            }

            if (this.targetView != null)
            {
                this.targetView.Dispose();
                this.targetView = null;
            }

            if(this.texture != null)
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
