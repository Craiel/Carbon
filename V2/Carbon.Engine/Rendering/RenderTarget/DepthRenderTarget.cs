using Carbon.Engine.Contracts.Logic;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace Carbon.Engine.Rendering.RenderTarget
{
    internal class DepthRenderTarget : RenderTargetBase
    {
        private Texture2D texture;
        private DepthStencilView targetView;
        private ShaderResourceView textureView;

        private BlendState blendState;

        private Texture2DDescription desiredTexture;
        private DepthStencilViewDescription desiredTargetView;
        private ShaderResourceViewDescription desiredShaderResourceView;

        private bool isResizing;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ShaderResourceView View
        {
            get
            {
                return this.textureView;
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

            graphics.ImmediateContext.ClearDepthStencilView(this.targetView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        public override void Set(ICarbonGraphics graphics)
        {
            if (this.isResizing)
            {
                return;
            }

            // Set the target views and viewport
            graphics.ImmediateContext.OutputMerger.SetTargets(this.targetView);
            graphics.ImmediateContext.Rasterizer.SetViewports(this.Viewport);

            base.Set(graphics);
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
                Format = Format.R24G8_Typeless,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };

            this.desiredTargetView = new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0
            };

            this.desiredShaderResourceView = new ShaderResourceViewDescription
            {
                Format = Format.R24_UNorm_X8_Typeless,
                Dimension = ShaderResourceViewDimension.Texture2D,
                MipLevels = this.desiredTexture.MipLevels,
                MostDetailedMip = 0
            };
            
            this.texture = graphics.StateManager.GetTexture(this.desiredTexture);
            this.targetView = graphics.StateManager.GetDepthStencilView(this.desiredTargetView, this.texture);
            this.textureView = new ShaderResourceView(graphics.ImmediateContext.Device, this.texture, this.desiredShaderResourceView);
            
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

            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
        }
    }
}
