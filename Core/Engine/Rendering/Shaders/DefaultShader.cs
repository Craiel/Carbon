namespace Core.Engine.Rendering.Shaders
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    using SlimDX;
    using SlimDX.D3DCompiler;
    using SlimDX.Direct3D11;

    public class DefaultShader : CarbonShader, IDefaultShader
    {
        private const int MaxDirectionalLights = 4;
        private const int MaxPointLights = 8;
        private const int MaxSpotLights = 4;

        private readonly ICarbonGraphics graphics;

        private readonly SlimDX.Direct3D11.Buffer[] buffers;
        private readonly SamplerState[] samplerStates;
        private readonly SamplerDescription[] samplerStateCache;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;

        private readonly int lightConstantBufferSize = Marshal.SizeOf(typeof(Vector4)) +
                                                       Marshal.SizeOf(typeof(Vector4)) +
                                                       Marshal.SizeOf(typeof(Vector4)) +
                                                       (Marshal.SizeOf(typeof(DirectionalLightData)) * MaxDirectionalLights) +
                                                       (Marshal.SizeOf(typeof(PointLightData)) * MaxPointLights) +
                                                       (Marshal.SizeOf(typeof(SpotLightData)) * MaxSpotLights);

        private readonly int materialConstantBufferSize = Marshal.SizeOf(typeof(MaterialConstantBuffer));
        
        private readonly SamplerDescription diffuseSamplerDescription;
        private readonly SamplerDescription normalSamplerDescription;
        private readonly SamplerDescription specularSamplerDescription;

        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;
        private LightConstantBuffer lightConstantBuffer;
        private MaterialConstantBuffer materialConstantBuffer;

        private bool reloadShaderState = true;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DefaultShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new SlimDX.Direct3D11.Buffer[4];
            this.resources = new ShaderResourceView[6];
            this.samplerStates = new SamplerState[3];
            this.samplerStateCache = new SamplerDescription[3];
            this.macros = new ShaderMacro[5];
            this.macros[0].Name = "DIFFUSEMAP";
            this.macros[1].Name = "NORMALMAP";
            this.macros[2].Name = "USECOLOR";
            this.macros[3].Name = "SPECULARMAP";
            this.macros[4].Name = "ALPHAMAP";

            this.SetFile("default.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");

            this.LightingEnabled = true;
            this.NormalMapEnabled = true;
            this.SpecularEnabled = true;
            this.AlphaEnabled = true;

            this.instanceConstantBuffer = new InstanceConstantBuffer { World = new Matrix[RenderInstruction.MaxInstanceCount] };
            this.lightConstantBuffer = new LightConstantBuffer
            {
                DirectionalData = new DirectionalLightData[MaxDirectionalLights],
                PointData = new PointLightData[MaxPointLights],
                SpotData = new SpotLightData[MaxSpotLights]
            };

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

            this.specularSamplerDescription = new SamplerDescription
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
        public bool NormalMapEnabled { get; set; }
        public bool SpecularEnabled { get; set; }
        public bool AlphaEnabled { get; set; }

        public Vector4 AmbientLight { get; set; }

        public void ClearLight()
        {
            this.lightConstantBuffer.DirectionalLights = 0;
            this.lightConstantBuffer.PointLights = 0;
            this.lightConstantBuffer.SpotLights = 0;

            for (int i = 0; i < MaxDirectionalLights; i++)
            {
                this.lightConstantBuffer.DirectionalData[i].Clear();
            }

            for (int i = 0; i < MaxPointLights; i++)
            {
                this.lightConstantBuffer.PointData[i].Clear();
            }

            for (int i = 0; i < MaxSpotLights; i++)
            {
                this.lightConstantBuffer.SpotData[i].Clear();
            }
        }

        public void AddDirectionalLight(Vector3 direction, Vector4 color, float specularPower = 1.0f)
        {
            if (this.lightConstantBuffer.DirectionalLights >= MaxDirectionalLights)
            {
                throw new InvalidOperationException("Maximum number of directional lights exceeded: " + MaxDirectionalLights);
            }

            var index = (int)this.lightConstantBuffer.DirectionalLights;
            this.lightConstantBuffer.DirectionalData[index].DiffuseColor = color;
            this.lightConstantBuffer.DirectionalData[index].Direction = new Vector4(direction, 1);
            this.lightConstantBuffer.DirectionalData[index].SpecularPower = specularPower;
            this.lightConstantBuffer.DirectionalLights++;
        }

        public void AddPointLight(Vector4 position, Vector4 color, float range, float specularPower = 1.0f)
        {
            if (this.lightConstantBuffer.PointLights >= MaxPointLights)
            {
                throw new InvalidOperationException("Maximum number of point lights exceeded: " + MaxPointLights);
            }

            var index = (int)this.lightConstantBuffer.PointLights;
            this.lightConstantBuffer.PointData[index].DiffuseColor = color;
            this.lightConstantBuffer.PointData[index].Position = position;
            this.lightConstantBuffer.PointData[index].Range = range;
            this.lightConstantBuffer.PointData[index].SpecularPower = specularPower;
            this.lightConstantBuffer.PointLights++;
        }

        public void AddSpotLight(Vector4 position, Vector3 direction, Vector4 color, float range, Vector2 angles, float specularPower = 1.0f)
        {
            if (this.lightConstantBuffer.SpotLights >= MaxSpotLights)
            {
                throw new InvalidOperationException("Maximum number of spot lights exceeded: " + MaxSpotLights);
            }

            var index = (int)this.lightConstantBuffer.SpotLights;
            this.lightConstantBuffer.SpotData[index].DiffuseColor = color;
            this.lightConstantBuffer.SpotData[index].Position = position;
            this.lightConstantBuffer.SpotData[index].Direction = new Vector4(direction, 1);
            this.lightConstantBuffer.SpotData[index].Range = range;
            this.lightConstantBuffer.SpotData[index].SpecularPower = specularPower;
            this.lightConstantBuffer.SpotData[index].Angles = new Vector2((float)Math.Cos(angles.X / 2.0f), (float)Math.Cos(angles.Y / 2.0f));
            this.lightConstantBuffer.SpotLights++;
        }

        public override void Permutate()
        {
            // Todo: Generate permutations for all macro combinations
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
                    if (instruction.Instances[i] == null)
                    {
                        throw new InvalidDataException("Instance data was null");
                    }

                    this.instanceConstantBuffer.World[i] = Matrix.Transpose((Matrix)instruction.Instances[i]);
                }

                this.SetConstantBufferData(1, this.InstanceConstantBufferSize, this.instanceConstantBuffer.World);
            }

            DataStream stream = this.BeginSetConstantBufferData(2);
            stream.Write(parameters.CameraPosition);
            stream.Write(this.AmbientLight);
            stream.Write(this.lightConstantBuffer.DirectionalLights);
            stream.Write(this.lightConstantBuffer.PointLights);
            stream.Write(this.lightConstantBuffer.SpotLights);
            stream.Write(0.0f); // Padding
            stream.WriteRange(this.lightConstantBuffer.DirectionalData);
            stream.WriteRange(this.lightConstantBuffer.PointData);
            stream.WriteRange(this.lightConstantBuffer.SpotData);
            this.EndSetConstantBufferData(2);

            this.SetConstantBufferData(3, this.materialConstantBufferSize, this.materialConstantBuffer);
        }

        private void ConfigureTextures(RenderInstruction instruction)
        {
            if (instruction.Color != null)
            {
                this.materialConstantBuffer.Color = (Vector4)instruction.Color;
            }

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

            if (this.specularSamplerDescription != this.samplerStateCache[2])
            {
                this.samplerStateCache[2] = this.specularSamplerDescription;
                this.samplerStates[2] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[2]);
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

            if (instruction.NormalTexture != null)
            {
                if (instruction.NormalTexture.View == null)
                {
                    instruction.NormalTexture.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[1] = instruction.NormalTexture.View;
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

            if (instruction.AlphaTexture != null)
            {
                if (instruction.AlphaTexture.View == null)
                {
                    instruction.AlphaTexture.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[5] = instruction.AlphaTexture.View;
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

            this.buffers[2] =
                this.graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.lightConstantBufferSize,
                        ResourceUsage.Default,
                        BindFlags.ConstantBuffer,
                        CpuAccessFlags.None,
                        ResourceOptionFlags.None,
                        0));

            this.buffers[3] =
                this.graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.materialConstantBufferSize,
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

            this.macros[0].Value = instruction.DiffuseTexture == null ? "0" : "1";
            this.macros[1].Value = instruction.NormalTexture == null || !this.NormalMapEnabled ? "0" : "1";
            this.macros[2].Value = instruction.Color == null ? "0" : "1";
            this.macros[3].Value = instruction.SpecularTexture == null || !this.SpecularEnabled ? "0" : "1";
            this.macros[4].Value = instruction.AlphaTexture == null || !this.AlphaEnabled ? "0" : "1";

            this.SetMacros(this.macros);
        }

        // -------------------------------------------------------------------
        // Structs
        // -------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MaterialConstantBuffer
        {
            public Vector4 Color;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LightConstantBuffer
        {
            public Vector4 CameraPosition;
            public float DirectionalLights;
            public float PointLights;
            public float SpotLights;
            public float Padding;
            public DirectionalLightData[] DirectionalData;
            public PointLightData[] PointData;
            public SpotLightData[] SpotData;
        }
    }
}
