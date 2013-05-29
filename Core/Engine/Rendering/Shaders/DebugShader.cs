using Core.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace Core.Engine.Rendering.Shaders
{
    public enum DebugShaderMode
    {
        Normal,
        Depth
    }

    public interface IDebugShader : ICarbonShader
    {
        DebugShaderMode Mode { get; set; }
    }

    public class DebugShader : CarbonShader, IDebugShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly SamplerState[] samplerStates;
        private readonly SamplerDescription[] samplerStateCache;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;
        
        private readonly SamplerDescription diffuseSamplerDescription;

        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;

        private bool reloadShaderState = true;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DebugShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[2];
            this.resources = new ShaderResourceView[1];
            this.samplerStates = new SamplerState[1];
            this.samplerStateCache = new SamplerDescription[1];
            this.macros = new ShaderMacro[3];
            this.macros[0].Name = "INSTANCED";
            this.macros[1].Name = "RENDERNORMALS";
            this.macros[2].Name = "RENDERDEPTH";

            this.SetFile("Debug.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");

            this.instanceConstantBuffer = new InstanceConstantBuffer { World = new Matrix[RenderInstruction.MaxInstanceCount] };
            
            this.diffuseSamplerDescription = new SamplerDescription
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
        // Public
        // -------------------------------------------------------------------
        public DebugShaderMode Mode { get; set; }

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
            this.defaultConstantBuffer.InvertedView = Matrix.Transpose(Matrix.Invert(parameters.View));
            this.defaultConstantBuffer.InvertedProjection = Matrix.Transpose(Matrix.Invert(parameters.Projection));
            this.defaultConstantBuffer.InvertedViewProjection = Matrix.Transpose(Matrix.Invert(parameters.View * parameters.Projection));

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

            if(samplerStateChanged)
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
            this.macros[1].Value = this.Mode != DebugShaderMode.Normal ? "0" : "1";
            this.macros[2].Value = this.Mode != DebugShaderMode.Depth ? "0" : "1";

            this.SetMacros(this.macros);
        }
    }
}
