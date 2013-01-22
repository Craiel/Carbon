using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;

using SlimDX;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Rendering.RenderTarget
{
    public enum RendertargetBlendMode
    {
        None,
        Additive,
        Alpha
    }

    public abstract class RenderTargetBase : IRenderTarget
    {
        private readonly int[] currentSize = new int[2];
        private readonly BlendStateDescription desiredBlendState;

        private RendertargetBlendMode blendMode;
        private bool needBlendStateUpdate;
        private BlendState blendState;
        
        protected RenderTargetBase()
        {
            this.desiredBlendState = new BlendStateDescription();
            this.desiredBlendState.RenderTargets[0].BlendEnable = true;
            this.desiredBlendState.RenderTargets[0].SourceBlend = BlendOption.One;
            this.desiredBlendState.RenderTargets[0].DestinationBlend = BlendOption.One;
            this.desiredBlendState.RenderTargets[0].BlendOperation = BlendOperation.Add;
            this.desiredBlendState.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            this.desiredBlendState.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;
            this.desiredBlendState.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            this.desiredBlendState.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

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

        public void Resize(ICarbonGraphics graphics, int width, int height)
        {
            if (currentSize[0] == width && currentSize[1] == height)
            {
                return;
            }

            this.Viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);

            this.DoResize(graphics, width, height);

            currentSize[0] = width;
            currentSize[1] = height;
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

        public virtual void Dispose()
        {
            if (this.blendState != null)
            {
                this.blendState.Dispose();
                this.blendState = null;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void DoResize(ICarbonGraphics graphics, int width, int height);

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
                        blendDescription.RenderTargets[0].BlendEnable = true;
                        blendDescription.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
                        blendDescription.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                        blendDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
                        blendDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
                        blendDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
                        blendDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
                        blendDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                        this.blendState = graphics.StateManager.GetBlendState(blendDescription);
                        break;
                    }

                case RendertargetBlendMode.Additive:
                    {
                        var blendDescription = new BlendStateDescription();
                        blendDescription.RenderTargets[0].BlendEnable = true;
                        blendDescription.RenderTargets[0].SourceBlend = BlendOption.One;
                        blendDescription.RenderTargets[0].DestinationBlend = BlendOption.One;
                        blendDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
                        blendDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
                        blendDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;
                        blendDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
                        blendDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                        this.blendState = graphics.StateManager.GetBlendState(blendDescription);
                        break;
                    }
            }

            this.needBlendStateUpdate = false;
        }
    }
}
