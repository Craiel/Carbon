namespace Core.Engine.Rendering.Shaders
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;

    public class BlendShader : CarbonShader, IBlendShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;
        
        private bool reloadShaderState = true;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BlendShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[1];
            this.resources = new ShaderResourceView[3];
            this.macros = new ShaderMacro[0];

            this.SetFile("Blend.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");
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
            this.ConfigureMacros();

            // Configure the Textures
            this.ConfigureTextures(instruction);

            // No need to send any matrix info here, this only blends
            this.SetConstantBufferData(0, this.DefaultConstantBufferSize, DefaultConstantBuffer.Empty);
        }

        private void ConfigureTextures(RenderInstruction instruction)
        {
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

            if (instruction.SpecularTexture != null)
            {
                if (instruction.SpecularTexture.View == null)
                {
                    instruction.SpecularTexture.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[2] = instruction.SpecularTexture.View;
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

        private void ConfigureMacros()
        {
            this.SetMacroDefaults();
            this.SetMacros(this.macros);
        }
    }
}
