using Core.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace Core.Engine.Rendering.Shaders
{
    public interface IShadowMapShader : ICarbonShader
    {
    }

    public class ShadowMapShader : CarbonShader, IShadowMapShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly ShaderMacro[] macros;
        
        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;

        private bool reloadShaderState = true;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ShadowMapShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[2];
            this.macros = new ShaderMacro[1];
            this.macros[0].Name = "INSTANCED";

            this.SetFile("ShadowMap.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");

            this.instanceConstantBuffer = new InstanceConstantBuffer { World = new Matrix[RenderInstruction.MaxInstanceCount] };
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
                this.macros[i].Value = "0";
            }
        }

        private void ConfigureMacros(RenderInstruction instruction)
        {
            this.SetMacroDefaults();

            this.macros[0].Value = instruction.InstanceCount <= 1 ? "0" : "1";

            this.SetMacros(this.macros);
        }
    }
}
