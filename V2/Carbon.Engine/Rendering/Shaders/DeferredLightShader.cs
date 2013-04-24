using System;
using System.Runtime.InteropServices;

using Carbon.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace Carbon.Engine.Rendering.Shaders
{
    public interface IDeferredLightShader : ICarbonShader
    {
        void SetAmbient(Vector4 color);
        void SetDirectional(Vector4 position, Vector3 direction, Vector4 color, Matrix lightViewProjection);
        void SetPoint(Vector4 position, Vector4 color, float range, Matrix lightViewProjection);
        void SetSpot(Vector4 position, Vector3 direction, Vector4 color, float range, Vector2 angles);
    }

    public class DeferredLightShader : CarbonShader, IDeferredLightShader
    {
        private readonly ICarbonGraphics graphics;

        private readonly Buffer[] buffers;
        private readonly SamplerState[] samplerStates;
        private readonly SamplerDescription[] samplerStateCache;
        private readonly ShaderResourceView[] resources;
        private readonly ShaderMacro[] macros;

        private readonly int lightConstantBufferSize = Marshal.SizeOf(typeof(LightConstantBuffer));
        private readonly int cameraConstantBufferSize = Marshal.SizeOf(typeof(CameraConstantBuffer));

        private readonly SamplerDescription diffuseSamplerDescription;
        private readonly SamplerDescription normalSamplerDescription;
        private readonly SamplerDescription specularSamplerDescription;
        private readonly SamplerDescription shadowMapSamplerDescription;

        private DefaultConstantBuffer defaultConstantBuffer;
        private InstanceConstantBuffer instanceConstantBuffer;
        private LightConstantBuffer lightConstantBuffer;
        private CameraConstantBuffer cameraConstantBuffer;

        private bool reloadShaderState = true;

        private bool isDirectional;
        private bool isPoint;
        private bool isSpot;
        private bool isAmbient;
        private bool isShadowMapping;
        private bool needLightPositionUpdate;

        private Vector3 lightPosition;
        private Vector3 lightDirection;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DeferredLightShader(ICarbonGraphics graphics)
            : base(graphics)
        {
            this.graphics = graphics;

            this.buffers = new Buffer[4];
            this.resources = new ShaderResourceView[5];
            this.samplerStates = new SamplerState[4];
            this.samplerStateCache = new SamplerDescription[4];
            this.macros = new ShaderMacro[7];
            this.macros[0].Name = "INSTANCED";
            this.macros[1].Name = "VOLUMERENDERING";
            this.macros[2].Name = "POINTLIGHT";
            this.macros[3].Name = "SPOTLIGHT";
            this.macros[4].Name = "DIRECTIONALLIGHT";
            this.macros[5].Name = "AMBIENTLIGHT";
            this.macros[6].Name = "SHADOWMAPPING";

            this.SetFile("DeferredLight.fx");
            this.SetEntryPoints("VS", "PS");
            this.SetProfiles("vs_4_0", "ps_4_0");
            
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

            this.shadowMapSamplerDescription = new SamplerDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void SetAmbient(Vector4 color)
        {
            this.ClearLight();
            this.isAmbient = true;

            this.lightConstantBuffer.Color = color;
        }

        public void SetDirectional(Vector4 position, Vector3 direction, Vector4 color, Matrix lightViewProjection)
        {
            this.ClearLight();
            this.isDirectional = true;
            this.isShadowMapping = true;

            this.lightPosition = new Vector3(position.X, position.Y, position.Z);
            this.lightDirection = direction;
            this.lightConstantBuffer.Color = color;
            this.lightConstantBuffer.LightViewProjection = lightViewProjection;
            this.needLightPositionUpdate = true;
        }

        public void SetPoint(Vector4 position, Vector4 color, float range, Matrix lightViewProjection)
        {
            this.ClearLight();
            this.isPoint = true;
            this.isShadowMapping = true;

            this.lightPosition = new Vector3(position.X, position.Y, position.Z);
            this.lightConstantBuffer.Color = color;
            this.lightConstantBuffer.Range = range;
            this.lightConstantBuffer.LightViewProjection = lightViewProjection;
            this.needLightPositionUpdate = true;
        }

        public void SetSpot(Vector4 position, Vector3 direction, Vector4 color, float range, Vector2 angles)
        {
            this.ClearLight();
            this.isSpot = true;

            this.lightDirection = direction;
            this.lightPosition = new Vector3(position.X, position.Y, position.Z);
            this.lightConstantBuffer.Color = color;
            this.lightConstantBuffer.Range = range;
            this.lightConstantBuffer.SpotlightAngles = new Vector2((float)Math.Cos(angles.X / 2.0f), (float)Math.Cos(angles.Y / 2.0f));
            this.needLightPositionUpdate = true;
        }

        // -------------------------------------------------------------------
        // Structs
        // -------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct CameraConstantBuffer
        {
            public Vector4 CameraPosition;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LightConstantBuffer
        {
            public Vector4 Color;
            public Vector4 Position;
            public Vector4 Direction;
            public Vector2 SpotlightAngles;

            public float Range;
            public float padding;

            public Vector2 ShadowMapSize;
            public Vector2 padding2;
            
            public Matrix LightViewProjection;

            public void Clear()
            {
                this.Color = Vector4.Zero;
                this.Position = Vector4.Zero;
                this.Direction = Vector4.Zero;
                this.SpotlightAngles = Vector2.Zero;
                this.ShadowMapSize = Vector2.Zero;
                this.LightViewProjection = Matrix.Identity;
                this.Range = 0;
            }
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
                    this.instanceConstantBuffer.World[i] = Matrix.Transpose((Matrix)instruction.Instances[i]);
                }

                this.SetConstantBufferData(1, this.InstanceConstantBufferSize, this.instanceConstantBuffer.World);
            }

            if (this.needLightPositionUpdate)
            {
                Vector4 position = new Vector4(this.lightPosition, 1.0f);
                Vector4 direction = new Vector4(this.lightDirection, 0);
                this.lightConstantBuffer.Direction = Vector4.Transform(direction, parameters.View);
                this.lightConstantBuffer.Position = Vector4.Transform(position, parameters.View);
                this.needLightPositionUpdate = false;
            }

            this.SetConstantBufferData(2, this.lightConstantBufferSize, this.lightConstantBuffer);

            this.cameraConstantBuffer.CameraPosition = parameters.CameraPosition;
            this.SetConstantBufferData(3, this.cameraConstantBufferSize, this.cameraConstantBuffer);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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

            if (this.specularSamplerDescription != this.samplerStateCache[2])
            {
                this.samplerStateCache[2] = this.specularSamplerDescription;
                this.samplerStates[2] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[2]);
                samplerStateChanged = true;
            }

            if (this.shadowMapSamplerDescription != this.samplerStateCache[3])
            {
                this.samplerStateCache[3] = this.shadowMapSamplerDescription;
                this.samplerStates[3] = this.graphics.StateManager.GetSamplerState(this.samplerStateCache[3]);
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

            if (instruction.DepthMap != null)
            {
                if (instruction.DepthMap.View == null)
                {
                    instruction.DepthMap.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[3] = instruction.DepthMap.View;
                texturesChanged = true;
            }

            if (instruction.ShadowMap != null)
            {
                if (instruction.ShadowMapSize == null)
                {
                    throw new InvalidOperationException();
                }

                if (instruction.ShadowMap.View == null)
                {
                    instruction.ShadowMap.InitializeView(this.graphics.ImmediateContext.Device);
                }

                this.resources[4] = instruction.ShadowMap.View;
                this.lightConstantBuffer.ShadowMapSize = (Vector2)instruction.ShadowMapSize;
                texturesChanged = true;
            }

            if (texturesChanged)
            {
                this.SetResources(this.resources);
            }
        }

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

            this.buffers[2] =
                graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.lightConstantBufferSize,
                        ResourceUsage.Default,
                        BindFlags.ConstantBuffer,
                        CpuAccessFlags.None,
                        ResourceOptionFlags.None,
                        0));

            this.buffers[3] =
                graphics.StateManager.GetBuffer(
                    new BufferDescription(
                        this.cameraConstantBufferSize,
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
            this.macros[2].Value = this.isPoint ? "1" : "0";
            this.macros[3].Value = this.isSpot ? "1" : "0";
            this.macros[4].Value = this.isDirectional ? "1" : "0";
            this.macros[5].Value = this.isAmbient ? "1" : "0";
            this.macros[6].Value = this.isShadowMapping ? "1" : "0";
            
            this.SetMacros(this.macros);
        }

        private void ClearLight()
        {
            this.isDirectional = false;
            this.isPoint = false;
            this.isSpot = false;
            this.isAmbient = false;
            this.isShadowMapping = false;

            this.lightConstantBuffer.Clear();
        }
    }
}
