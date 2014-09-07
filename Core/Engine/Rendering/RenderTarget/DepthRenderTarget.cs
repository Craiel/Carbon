namespace Core.Engine.Rendering.RenderTarget
{
    using System.IO;

    using CarbonCore.UtilsDX;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;

    internal class DepthRenderTarget : RenderTargetBase
    {
        private TextureData texture;
        private DepthStencilView targetView;

        private BlendState blendState;

        private Texture2DDescription desiredTexture;
        private DepthStencilViewDescription desiredTargetView;
        private ShaderResourceViewDescription desiredShaderResourceView;
        private ImageLoadInformation loadInformation;

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

        public ImageLoadInformation LoadInformation
        {
            get
            {
                return this.loadInformation;
            }
        }

        public ShaderResourceViewDescription ViewDescription
        {
            get
            {
                return this.desiredShaderResourceView;
            }
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
            graphics.ImmediateContext.Rasterizer.SetViewport(this.Viewport);

            base.Set(graphics);
        }

        public void StoreData(DeviceContext context, Stream target, ImageFileFormat format = ImageFileFormat.Dds)
        {
            if (!this.isResizing && this.texture != null)
            {
                ResourceUtils.ResourceToStream(context, this.texture.Texture2D, format, target);
            }
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
                Format = Format.R24G8_Typeless,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };

            this.loadInformation = new ImageLoadInformation
                                       {
                                           Width = this.desiredTexture.Width,
                                           Height = this.desiredTexture.Height,
                                           MipLevels = this.desiredTexture.MipLevels,
                                           Format = Format.R24G8_Typeless,
                                           Usage = ResourceUsage.Default,
                                           BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                                           CpuAccessFlags = CpuAccessFlags.None,
                                           OptionFlags = ResourceOptionFlags.None
                                       };

            this.desiredTargetView = new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
            };

            this.desiredShaderResourceView = new ShaderResourceViewDescription
            {
                Format = Format.R24_UNorm_X8_Typeless,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                                {
                                    MipLevels = this.desiredTexture.MipLevels,
                                    MostDetailedMip = 0
                                }
            };
            
            Texture2D data = graphics.StateManager.GetTexture(this.desiredTexture);
            var view = new ShaderResourceView(graphics.ImmediateContext.Device, data, this.desiredShaderResourceView);
            this.targetView = graphics.StateManager.GetDepthStencilView(this.desiredTargetView, data);
            this.texture = new TextureData(data, view);
            
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
        }
    }
}
