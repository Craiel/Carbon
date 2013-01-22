using Carbon.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Rendering.RenderTarget
{
    class BackBufferRenderTarget : RenderTargetBase
    {
        private Texture2D depthStencil;
        private DepthStencilView depthStencilView;

        private Texture2DDescription desiredDepthStencil;
        private DepthStencilViewDescription desiredDepthStencilView;

        private bool isResizing;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Dispose()
        {
            this.DisposeResources();
        }
        
        public override void Clear(ICarbonGraphics graphics, Vector4 color)
        {
            if (this.isResizing)
            {
                return;
            }

            graphics.ImmediateContext.ClearRenderTargetView(graphics.BackBufferView, new Color4(color));
            graphics.ImmediateContext.ClearDepthStencilView(this.depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public override void Set(ICarbonGraphics graphics)
        {
            if (this.isResizing)
            {
                return;
            }

            // Set the target views and viewport
            graphics.ImmediateContext.OutputMerger.SetTargets(this.depthStencilView, graphics.BackBufferView);
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

            this.depthStencil = graphics.StateManager.GetTexture(this.desiredDepthStencil);
            this.depthStencilView = graphics.StateManager.GetDepthStencilView(this.desiredDepthStencilView, this.depthStencil);

            this.isResizing = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DisposeResources()
        {
            if(this.depthStencilView != null)
            {
                this.depthStencilView.Dispose();
                this.depthStencilView = null;
            }

            if(this.depthStencil != null)
            {
                this.depthStencil.Dispose();
                this.depthStencil = null;
            }
        }
    }
}
