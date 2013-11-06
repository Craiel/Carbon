namespace Core.Engine.Rendering.Shaders
{
    using System.IO;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;

    public class PlainShader : CarbonShader, IPlainShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly SamplerState[] samplerStates;
        private readonly SamplerStateDescription[] samplerStateCache;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;

        private readonly SamplerStateDescription diffuseSamplerDescription;

        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;

        private bool reloadShaderState = true;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PlainShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[2];
            this.resources = new ShaderResourceView[1];
            this.samplerStates = new SamplerState[1];
            this.samplerStateCache = new SamplerStateDescription[1];
            this.macros = new ShaderMacro[1];
            this.macros[0].Name = "INSTANCED";

            this.SetFile("Plain.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");

            this.instanceConstantBuffer = new InstanceConstantBuffer { World = new Matrix[RenderInstruction.MaxInstanceCount] };

            this.diffuseSamplerDescription = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = 0
            };
        }
        
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
            this.defaultConstantBuffer.World = instruction.World;
            this.defaultConstantBuffer.View = parameters.View;
            this.defaultConstantBuffer.Projection = parameters.Projection;

            this.SetConstantBufferData(0, this.DefaultConstantBufferSize, this.defaultConstantBuffer);

            if (instruction.InstanceCount > 1)
            {
                for (int i = 0; i < instruction.InstanceCount; i++)
                {
                    if (instruction.Instances[i] == null)
                    {
                        throw new InvalidDataException("Instance data is null");
                    }

                    this.instanceConstantBuffer.World[i] = (Matrix)instruction.Instances[i];
                }

                this.SetConstantBufferData(1, this.InstanceConstantBufferSize, this.instanceConstantBuffer.World);
            }
        }

        private void ConfigureTextures(RenderInstruction instruction)
        {
            // Configure the Sampling State
            bool samplerStateChanged = false;
            if (this.diffuseSamplerDescription.Equals(this.samplerStateCache[0]))
            {
                this.samplerStateCache[0] = this.diffuseSamplerDescription;
                this.samplerStates[0] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[0]);
                samplerStateChanged = true;
            }

            if (samplerStateChanged)
            {
                this.SetSamplerStates(this.samplerStates);
            }

            // Configure the Textures
            bool texturesChanged = false;
            if (instruction.DiffuseTexture != null)
            {
                if (instruction.DiffuseTexture.View == null)
                {
                    instruction.DiffuseTexture.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[0] = instruction.DiffuseTexture.View;
                texturesChanged = true;
            }

            if (texturesChanged)
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
                this.graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.DefaultConstantBufferSize,
                        ResourceUsage.Default,
                        BindFlags.ConstantBuffer,
                        CpuAccessFlags.None,
                        ResourceOptionFlags.None,
                        0));

            this.buffers[1] =
                this.graphics.StateManager.GetBuffer(
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
            for (int i = 0; i < this.macros.Length; i++)
            {
                this.macros[i].Definition = "0";
            }
        }

        private void ConfigureMacros(RenderInstruction instruction)
        {
            this.SetMacroDefaults();

            this.macros[0].Definition = instruction.InstanceCount <= 1 ? "0" : "1";

            this.SetMacros(this.macros);
        }
    }
}
