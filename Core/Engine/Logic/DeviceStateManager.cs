namespace Core.Engine.Logic
{
    using System;
    using System.Collections;

    using Core.Engine.Rendering;

    using SlimDX.D3DCompiler;
    using SlimDX.Direct3D11;

    public class DeviceStateManager : IDisposable
    {
        public static readonly RasterizerStateDescription WireFrameRasterDescription = new RasterizerStateDescription { FillMode = FillMode.Wireframe, CullMode = CullMode.None, IsDepthClipEnabled = true };
        public static readonly RasterizerStateDescription SolidRasterState = new RasterizerStateDescription { CullMode = CullMode.Back, FillMode = FillMode.Solid, IsDepthClipEnabled = true };
        public static readonly DepthStencilStateDescription DefaultDepthStencilState = new DepthStencilStateDescription
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
            StencilReadMask = 0xFF,
            StencilWriteMask = 0xFF,
            FrontFace = new DepthStencilOperationDescription
            {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Increment,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            },
            BackFace = new DepthStencilOperationDescription
            {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Decrement,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            }
        };

        private readonly Device device;

        private readonly Hashtable samplingStateCache;
        private readonly Hashtable rasterStateCache;
        private readonly Hashtable depthStencilStateCache;
        private readonly Hashtable bufferCache;
        private readonly Hashtable inputLayoutCache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DeviceStateManager(Device device)
        {
            this.device = device;

            this.samplingStateCache = new Hashtable(4096);
            this.rasterStateCache = new Hashtable(4096);
            this.depthStencilStateCache = new Hashtable(4096);
            this.bufferCache = new Hashtable(4096);
            this.inputLayoutCache = new Hashtable(1024);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            foreach (SamplerState state in this.samplingStateCache.Values)
            {
                state.Dispose();
            }

            this.samplingStateCache.Clear();

            foreach (RasterizerState state in this.rasterStateCache.Values)
            {
                state.Dispose();
            }

            this.rasterStateCache.Clear();

            foreach (DepthStencilState state in this.depthStencilStateCache.Values)
            {
                state.Dispose();
            }

            this.depthStencilStateCache.Clear();

            foreach (SlimDX.Direct3D11.Buffer buffer in this.bufferCache.Values)
            {
                buffer.Dispose();
            }

            this.bufferCache.Clear();

            foreach (InputLayout layout in this.inputLayoutCache.Values)
            {
                layout.Dispose();
            }

            this.inputLayoutCache.Clear();
        }
        
        public SamplerState GetSamplerState(SamplerDescription description)
        {
            if (!this.samplingStateCache.ContainsKey(description))
            {
                this.samplingStateCache.Add(description, SamplerState.FromDescription(this.device, description));
            }

            return (SamplerState)this.samplingStateCache[description];
        }

        public SlimDX.Direct3D11.Buffer GetBuffer(BufferDescription description)
        {
            if (!this.bufferCache.ContainsKey(description))
            {
                this.bufferCache.Add(description, new SlimDX.Direct3D11.Buffer(this.device, description));
            }

            return (SlimDX.Direct3D11.Buffer)this.bufferCache[description];
        }

        public InputLayout GetInputLayout(ShaderInputLayoutDescription description, ShaderSignature signature)
        {
            if (!InputStructures.InputLayouts.ContainsKey(description.Type))
            {
                throw new InvalidOperationException("No Input layout definition for Type " + description.Type);
            }

            if (!this.inputLayoutCache.ContainsKey(description))
            {
                this.inputLayoutCache.Add(
                    description,
                    new InputLayout(this.device, signature, InputStructures.InputLayouts[description.Type]));
            }

            return (InputLayout)this.inputLayoutCache[description];
        }

        public RasterizerState GetRasterizerState(RasterizerStateDescription description)
        {
            if (!this.rasterStateCache.ContainsKey(description))
            {
                this.rasterStateCache.Add(description, RasterizerState.FromDescription(this.device, description));
            }

            return (RasterizerState)this.rasterStateCache[description];
        }

        public DepthStencilState GetDepthStencilState(DepthStencilStateDescription description)
        {
            if (!this.depthStencilStateCache.ContainsKey(description))
            {
                this.depthStencilStateCache.Add(description, DepthStencilState.FromDescription(this.device, description));
            }

            return (DepthStencilState)this.depthStencilStateCache[description];
        }

        public Texture2D GetTexture(Texture2DDescription description)
        {
            return new Texture2D(this.device, description);
        }

        public DepthStencilView GetDepthStencilView(DepthStencilViewDescription description, Texture2D texture)
        {
            return new DepthStencilView(this.device, texture, description);
        }

        public DynamicBuffer GetDynamicBuffer(BindFlags bindFlags)
        {
            return new DynamicBuffer(this.device, bindFlags);
        }

        public RenderTargetView GetRenderTargetView(Texture2D resource, RenderTargetViewDescription description)
        {
            return new RenderTargetView(this.device, resource, description);
        }

        public BlendState GetBlendState(BlendStateDescription description)
        {
            return BlendState.FromDescription(this.device, description);
        }
    }
}
