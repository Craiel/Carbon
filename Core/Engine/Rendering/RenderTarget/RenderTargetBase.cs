namespace Core.Engine.Rendering.RenderTarget
{
    using System;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;

    using SharpDX;
    using SharpDX.Direct3D11;

    public enum RendertargetBlendMode
    {
        None,
        Additive,
        Alpha
    }

    public abstract class RenderTargetBase : IRenderTarget
    {
        private readonly BlendStateDescription desiredBlendState;

        private TypedVector2<int> currentSize;

        private RendertargetBlendMode blendMode;
        private bool needBlendStateUpdate;
        private BlendState blendState;
        
        protected RenderTargetBase()
        {
            this.desiredBlendState = new BlendStateDescription();
            this.desiredBlendState.RenderTarget[0] = new RenderTargetBlendDescription
                                                         {
                                                             IsBlendEnabled = true,
                                                             SourceBlend = BlendOption.One,
                                                             DestinationBlend = BlendOption.One,
                                                             BlendOperation = BlendOperation.Add,
                                                             SourceAlphaBlend = BlendOption.One,
                                                             DestinationAlphaBlend = BlendOption.One,
                                                             AlphaBlendOperation = BlendOperation.Add,
                                                             RenderTargetWriteMask = ColorWriteMaskFlags.All
                                                         };


            /*this.desiredBlendState = new BlendStateDescription();
            this.desiredBlendState.RenderTargets[0].BlendEnable = this.isBlending;
            this.desiredBlendState.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            this.desiredBlendState.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            this.desiredBlendState.RenderTargets[0].BlendOperation = BlendOperation.Add;
            this.desiredBlendState.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            this.desiredBlendState.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
            this.desiredBlendState.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            this.desiredBlendState.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;*/
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Viewport Viewport { get; protected set; }

        public RendertargetBlendMode BlendMode
        {
            get
            {
                return this.blendMode;
            }

            set
            {
                if (this.blendMode != value)
                {
                    this.blendMode = value;
                    this.needBlendStateUpdate = true;
                }
            }
        }

        public void Resize(ICarbonGraphics graphics, TypedVector2<int> size)
        {
            if (this.currentSize == size)
            {
                return;
            }

            this.Viewport = new Viewport(0, 0, size.X, size.Y, 0.0f, 1.0f);

            this.DoResize(graphics, size);

            this.currentSize = size;
        }

        public abstract void Clear(ICarbonGraphics graphics, Vector4 color);

        public virtual void Set(ICarbonGraphics graphics)
        {
            if (this.needBlendStateUpdate)
            {
                this.UpdateBlendState(graphics);
            }

            graphics.ImmediateContext.OutputMerger.BlendState = this.blendState;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.blendState != null)
            {
                this.blendState.Dispose();
                this.blendState = null;
            }
        }

        protected abstract void DoResize(ICarbonGraphics graphics, TypedVector2<int> size);

        private void UpdateBlendState(ICarbonGraphics graphics)
        {
            if (this.blendState != null)
            {
                this.blendState.Dispose();
                this.blendState = null;
            }

            switch (this.blendMode)
            {
                case RendertargetBlendMode.None:
                    {
                        break;
                    }

                case RendertargetBlendMode.Alpha:
                    {
                        var blendDescription = new BlendStateDescription();
                        blendDescription.RenderTarget[0] = new RenderTargetBlendDescription
                                                               {
                                                                   IsBlendEnabled = true,
                                                                   SourceBlend = BlendOption.SourceAlpha,
                                                                   DestinationBlend = BlendOption.InverseSourceAlpha,
                                                                   BlendOperation = BlendOperation.Add,
                                                                   SourceAlphaBlend = BlendOption.One,
                                                                   DestinationAlphaBlend = BlendOption.Zero,
                                                                   AlphaBlendOperation = BlendOperation.Add,
                                                                   RenderTargetWriteMask = ColorWriteMaskFlags.All
                                                               };
                        this.blendState = graphics.StateManager.GetBlendState(blendDescription);
                        break;
                    }

                case RendertargetBlendMode.Additive:
                    {
                        var blendDescription = new BlendStateDescription();
                        blendDescription.RenderTarget[0] = new RenderTargetBlendDescription
                                                               {
                                                                   IsBlendEnabled = true,
                                                                   SourceBlend = BlendOption.One,
                                                                   DestinationBlend = BlendOption.One,
                                                                   BlendOperation = BlendOperation.Add,
                                                                   SourceAlphaBlend = BlendOption.One,
                                                                   DestinationAlphaBlend = BlendOption.One,
                                                                   AlphaBlendOperation = BlendOperation.Add,
                                                                   RenderTargetWriteMask = ColorWriteMaskFlags.All
                                                               };
                        this.blendState = graphics.StateManager.GetBlendState(blendDescription);
                        break;
                    }
            }

            this.needBlendStateUpdate = false;
        }
    }
}
