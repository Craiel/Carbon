using System;
using System.Runtime.InteropServices;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;

using Core.Utils;
using Core.Utils.Contracts;
using Core.Utils.Diagnostics;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

using Buffer = SlimDX.Direct3D11.Buffer;

namespace Carbon.Engine.Rendering
{
    public interface ICarbonShader : IEngineComponent
    {
        bool LightingEnabled { get; set; }

        void Permutate();

        void Apply(DeviceContext context, Type vertexType, RenderParameters parameters, RenderInstruction instruction);

        void ResetConfigurationState(DeviceContext context);
    }

    public abstract class CarbonShader : EngineComponent, ICarbonShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly CarbonShaderDescription desiredVertexShader;
        private readonly CarbonShaderDescription desiredPixelShader;
        private readonly ShaderInputLayoutDescription desiredInputLayout;

        private readonly ShaderMacro[] globalMacros;
        
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private ShaderSignature signature;
        private InputLayout inputLayout;

        private Buffer[] constantBuffers;
        private DataStream[] constantBufferData;
        private bool[] constantBufferDataState;
        private SamplerState[] samplerStates;
        private ShaderResourceView[] resources;
        private ShaderMacro[] combinedMacros;

        private bool reloadShaders = true;
        private bool uploadShaders = true;
        private bool uploadLayout = true;
        private bool uploadSamplers;
        private bool uploadBuffers;
        private bool uploadResources;
        
        private Type currentVertexType;

        int macroHash;

        internal readonly int DefaultConstantBufferSize = Marshal.SizeOf(typeof(DefaultConstantBuffer));
        internal readonly int InstanceConstantBufferSize = Marshal.SizeOf(typeof(Matrix)) * RenderInstruction.MaxInstanceCount;
        internal readonly int DirectionalLightDataSize = Marshal.SizeOf(typeof(DirectionalLightData));
        internal readonly int PointLightDataSize = Marshal.SizeOf(typeof(PointLightData));

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DefaultConstantBuffer
        {
            public Matrix World;
            public Matrix View;
            public Matrix Projection;
            public Matrix InvertedProjection;

            public Vector4 Padding; // Still no idea why this is needed...
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct InstanceConstantBuffer
        {
            public Matrix[] World;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DirectionalLightData
        {
            public Vector4 DiffuseColor;
            public Vector4 Direction;
            public float SpecularPower;
            public Vector3 Padding;

            public void Clear()
            {
                this.DiffuseColor = Vector4.Zero;
                this.Direction = Vector4.Zero;
                this.SpecularPower = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct PointLightData
        {
            public Vector4 DiffuseColor;
            public Vector4 Position;
            public float Range;
            public float SpecularPower;
            public Vector2 Padding;

            public void Clear()
            {
                this.DiffuseColor = Vector4.Zero;
                this.Position = Vector4.Zero;
                this.Range = 0;
                this.SpecularPower = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal  struct SpotLightData
        {
            public Vector4 DiffuseColor;
            public Vector4 Position;
            public Vector4 Direction;
            public float Range;
            public float SpecularPower;
            public Vector2 Angles;

            public void Clear()
            {
                this.DiffuseColor = Vector4.Zero;
                this.Position = Vector4.Zero;
                this.Direction = Vector4.Zero;
                this.Range = 0;
                this.SpecularPower = 0;
                this.Angles = Vector2.Zero;
            }
        }

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CarbonShader(ICarbonGraphics graphics)
        {
            this.graphics = graphics;

            this.desiredVertexShader = new CarbonShaderDescription();
            this.desiredPixelShader = new CarbonShaderDescription();
            this.desiredInputLayout = new ShaderInputLayoutDescription(this.desiredVertexShader);

            this.globalMacros = new ShaderMacro[2];
            this.globalMacros[0].Name = "INSTANCED";
            this.globalMacros[1].Name = "LIGHTING";

            this.LightingEnabled = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool LightingEnabled { get; set; }

        public virtual void Permutate()
        {
        }

        public void Apply(DeviceContext context, Type vertexType, RenderParameters parameters, RenderInstruction instruction)
        {
            // Analyze the instruction and set the macros we may need
            using (new ProfileRegion("configure shader configuration"))
            {
                this.Configure(parameters, instruction);
            }

            // Reload
            if (this.reloadShaders)
            {
                this.vertexShader = this.graphics.ShaderManager.GetVertexShader(this.desiredVertexShader);
                this.pixelShader = this.graphics.ShaderManager.GetPixelShader(this.desiredPixelShader);

                this.signature = this.graphics.ShaderManager.GetShaderSignature(this.desiredVertexShader);

                this.reloadShaders = false;
                this.uploadShaders = true;
            }

            // If the shaders where reloaded or the vertex type changed we have to get a different input layout
            if (this.reloadShaders || this.currentVertexType != vertexType)
            {
                this.desiredInputLayout.Type = vertexType;
                this.inputLayout = this.graphics.StateManager.GetInputLayout(this.desiredInputLayout, signature);
                this.currentVertexType = vertexType;
                this.uploadLayout = true;
            }

            using (new ProfileRegion("Upload shader configuration"))
            {
                this.UploadConfiguration(context);
            }
        }
        
        public override void Update(ITimer gameTime)
        {
        }

        public virtual void ResetConfigurationState(DeviceContext context)
        {
            context.VertexShader.Set(null);
            context.PixelShader.Set(null);

            context.InputAssembler.InputLayout = null;

            this.SetUploadAll();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void SetUploadAll()
        {
            this.uploadShaders = true;
            this.uploadLayout = true;
            this.uploadBuffers = this.constantBuffers != null;
            this.uploadSamplers = this.samplerStates != null;
            this.uploadResources = this.resources != null;

            if (this.constantBufferDataState != null)
            {
                for (int i = 0; i < this.constantBufferDataState.Length; i++)
                {
                    this.constantBufferDataState[i] = this.constantBufferData[i] != null;
                }
            }
        }

        protected void SetFile(string file)
        {
            this.desiredVertexShader.File = file;
            this.desiredPixelShader.File = file;

            this.reloadShaders = true;
        }

        protected void SetProfiles(string vertex, string pixel)
        {
            this.desiredVertexShader.Profile = vertex;
            this.desiredPixelShader.Profile = pixel;

            this.reloadShaders = true;
        }

        protected void SetEntryPoints(string vertex, string pixel)
        {
            this.desiredVertexShader.Entry = vertex;
            this.desiredPixelShader.Entry = pixel;

            this.reloadShaders = true;
        }

        protected void SetShaderFlags(ShaderFlags vertex, ShaderFlags pixel)
        {
            this.desiredVertexShader.ShaderFlags = vertex;
            this.desiredPixelShader.ShaderFlags = pixel;

            this.reloadShaders = true;
        }

        protected void SetEffectFlags(EffectFlags vertex, EffectFlags pixel)
        {
            this.desiredVertexShader.EffectFlags = vertex;
            this.desiredPixelShader.EffectFlags = pixel;

            this.reloadShaders = true;
        }

        protected void SetMacros(ShaderMacro[] localMacros)
        {
            if (localMacros != null)
            {
                this.combinedMacros = new ShaderMacro[localMacros.Length + this.globalMacros.Length];
                this.globalMacros.CopyTo(this.combinedMacros, 0);
                Array.Copy(localMacros, 0, this.combinedMacros, this.globalMacros.Length, localMacros.Length);
            }
            else
            {
                this.combinedMacros = this.globalMacros;
            }

            int newHash = HashUtils.CombineHashes(this.combinedMacros);
            if (this.macroHash != newHash)
            {
                this.desiredVertexShader.Macros = this.combinedMacros;
                this.desiredPixelShader.Macros = this.combinedMacros;

                this.reloadShaders = true;
                this.macroHash = newHash;
            }
        }

        protected void SetSamplerStates(SamplerState[] states)
        {
            this.samplerStates = states;
            this.uploadSamplers = states != null;
        }

        protected void SetConstantBuffers(Buffer[] buffers)
        {
            this.constantBuffers = buffers;
            if (buffers != null)
            {
                this.ResetConstantBufferData();
            }

            this.uploadBuffers = buffers != null;
        }

        protected DataStream BeginSetConstantBufferData(int id)
        {
            this.constantBufferDataState[id] = true;
            this.constantBufferData[id].Position = 0;
            return this.constantBufferData[id];
        }

        protected void EndSetConstantBufferData(int id)
        {
            this.constantBufferData[id].Position = 0;
        }

        protected void SetConstantBufferData<T>(int id, int size, T[] data)
            where T : struct
        {
            this.constantBufferDataState[id] = true;
            this.constantBufferData[id].Position = 0;
            this.constantBufferData[id].WriteRange(data);
            this.constantBufferData[id].Position = 0;
        }

        protected void SetConstantBufferData<T>(int id, int size, T data)
            where T : struct
        {
            this.constantBufferDataState[id] = true;
            this.constantBufferData[id].Position = 0;
            this.constantBufferData[id].Write(data);
            this.constantBufferData[id].Position = 0;
        }

        protected void SetResources(ShaderResourceView[] shaderResources)
        {
            this.resources = shaderResources;
            this.uploadResources = true;
        }

        protected virtual void Configure(RenderParameters parameters, RenderInstruction instruction)
        {
            this.ConfigureMacros(instruction);
        }

        protected void SetMacroDefaults(ShaderMacro[] macros)
        {
            for (int i = 0; i < macros.Length; i++)
            {
                macros[i].Value = "0";
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ResetConstantBufferData()
        {
            if (this.constantBufferData != null)
            {
                for (int i = 0; i < this.constantBufferData.Length; i++)
                {
                    this.constantBufferData[i].Dispose();
                }
            }

            // Change our data holders if the buffers changed
            this.constantBufferData = new DataStream[this.constantBuffers.Length];

            for (int i = 0; i < this.constantBufferData.Length; i++)
            {
                if (this.constantBuffers[i] == null)
                {
                    continue;
                }

                this.constantBufferData[i] = new DataStream(this.constantBuffers[i].Description.SizeInBytes, true, true);
            }

            this.constantBufferDataState = new bool[this.constantBufferData.Length];
        }

        private void UploadConfiguration(DeviceContext context)
        {
            if (this.uploadShaders)
            {
                context.VertexShader.Set(this.vertexShader);
                context.PixelShader.Set(this.pixelShader);
                this.uploadShaders = false;
            }

            if (this.uploadLayout)
            {
                context.InputAssembler.InputLayout = this.inputLayout;
                this.uploadLayout = false;
            }

            if (this.uploadBuffers)
            {
                context.VertexShader.SetConstantBuffers(this.constantBuffers, 0, this.constantBuffers.Length);
                context.PixelShader.SetConstantBuffers(this.constantBuffers, 0, this.constantBuffers.Length);
                this.uploadBuffers = false;
            }

            // Update the buffer data if it was set
            for (int i = 0; i < this.constantBufferData.Length; i++)
            {
                if (this.constantBufferDataState[i])
                {
                    context.UpdateSubresource(new DataBox(0, 0, this.constantBufferData[i]), this.constantBuffers[i], 0);
                    this.constantBufferDataState[i] = false;
                }
            }

            if (this.uploadSamplers)
            {
                context.VertexShader.SetSamplers(this.samplerStates, 0, this.samplerStates.Length);
                context.PixelShader.SetSamplers(this.samplerStates, 0, this.samplerStates.Length);
                this.uploadSamplers = false;
            }

            if (this.uploadResources)
            {
                context.VertexShader.SetShaderResources(this.resources, 0, this.resources.Length);
                context.PixelShader.SetShaderResources(this.resources, 0, this.resources.Length);
                this.uploadResources = false;
            }
        }

        private void ConfigureMacros(RenderInstruction instruction)
        {
            this.SetMacroDefaults(this.globalMacros);

            this.globalMacros[0].Value = instruction.InstanceCount <= 1 ? "0" : "1";
            this.globalMacros[1].Value = this.LightingEnabled ? "1" : "0";

            this.SetMacros(null);
        }
    }
}
