using Carbon.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace Carbon.Engine.Rendering.Shaders
{
    public interface IGBufferShader : ICarbonShader
    {
    }

    public class GBufferShader : CarbonShader, IGBufferShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly SamplerState[] samplerStates;
        private readonly SamplerDescription[] samplerStateCache;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;

        private readonly SamplerDescription diffuseSamplerDescription;
        private readonly SamplerDescription normalSamplerDescription;

        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;

        private bool reloadShaderState = true;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GBufferShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[2];
            this.resources = new ShaderResourceView[2];
            this.samplerStates = new SamplerState[2];
            this.samplerStateCache = new SamplerDescription[2];
            this.macros = new ShaderMacro[2];
            this.macros[0].Name = "INSTANCED";
            this.macros[1].Name = "NORMALMAP";

            this.SetFile("GBufferShader.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");

            this.AlphaEnabled = true;

            this.instanceConstantBuffer = new InstanceConstantBuffer { World = new Matrix[RenderInstruction.MaxInstanceCount] };

            this.diffuseSamplerDescription = new SamplerDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    ComparisonFunction = Comparison.Never,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };

            this.normalSamplerDescription = new SamplerDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    ComparisonFunction = Comparison.Never,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool AlphaEnabled { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Configure(RenderParameters parameters, RenderInstruction instruction)
        {
            base.Configure(parameters, instruction);

            if (this.reloadShaderState)
            {
                this.AcquireShaderStates();
            }

            // Evaluate the macros
            this.ConfigureMacros(instruction);

            // Configure the Textures
            this.ConfigureTextures(instruction);

            // Finalize the default buffer
            this.defaultConstantBuffer.World = Matrix.Transpose(instruction.World);
            this.defaultConstantBuffer.View = Matrix.Transpose(parameters.View);
            this.defaultConstantBuffer.Projection = Matrix.Transpose(parameters.Projection);
            this.defaultConstantBuffer.InvertedProjection = Matrix.Transpose(Matrix.Invert(parameters.Projection));

            this.SetConstantBufferData(0, this.DefaultConstantBufferSize, this.defaultConstantBuffer);
            
            if (instruction.InstanceCount > 1)
            {
                for (int i = 0; i < instruction.InstanceCount; i++)
                {
                    this.instanceConstantBuffer.World[i] = Matrix.Transpose((Matrix)instruction.Instances[i]);
                }

                this.SetConstantBufferData(1, this.InstanceConstantBufferSize, this.instanceConstantBuffer.World);
            }
        }

        private void ConfigureTextures(RenderInstruction instruction)
        {
            // Configure the Sampling State
            bool samplerStateChanged = false;
            if (this.diffuseSamplerDescription != this.samplerStateCache[0])
            {
                this.samplerStateCache[0] = this.diffuseSamplerDescription;
                this.samplerStates[0] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[0]);
                samplerStateChanged = true;
            }

            if (this.normalSamplerDescription != this.samplerStateCache[1])
            {
                this.samplerStateCache[1] = this.normalSamplerDescription;
                this.samplerStates[1] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[1]);
                samplerStateChanged = true;
            }

            if(samplerStateChanged)
            {
                this.SetSamplerStates(this.samplerStates);
            }

            // Configure the Textures
            bool texturesChanged = false;
            if (instruction.DiffuseTexture != this.resources[0])
            {
                this.resources[0] = instruction.DiffuseTexture;
                texturesChanged = true;
            }

            if (instruction.NormalTexture != this.resources[1])
            {
                this.resources[1] = instruction.NormalTexture;
                texturesChanged = true;
            }

            if(texturesChanged)
            {
                this.SetResources(this.resources);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AcquireShaderStates()
        {
            this.buffers[0] =
                graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.DefaultConstantBufferSize,
                        ResourceUsage.Default,
                        BindFlags.ConstantBuffer,
                        CpuAccessFlags.None,
                        ResourceOptionFlags.None,
                        0));
            
            this.buffers[1] =
                graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.InstanceConstantBufferSize,
                        ResourceUsage.Default,
                        BindFlags.ConstantBuffer,
                        CpuAccessFlags.None,
                        ResourceOptionFlags.None,
                        0));

            this.SetConstantBuffers(this.buffers);
            this.reloadShaderState = false;
        }

        private void SetMacroDefaults()
        {
            for (int i = 0; i < this.macros.Length;i++ )
            {
                this.macros[i].Value = "0";
            }
        }

        private void ConfigureMacros(RenderInstruction instruction)
        {
            this.SetMacroDefaults();

            this.macros[0].Value = instruction.InstanceCount <= 1 ? "0" : "1";
            this.macros[1].Value = instruction.NormalTexture == null ? "0" : "1";

            this.SetMacros(this.macros);
        }
    }
}
