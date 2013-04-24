using System;
using System.Collections.Generic;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering.Shaders;

using Core.Utils;

using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace Carbon.Engine.Rendering
{    
    public class Renderer : EngineComponent, IRenderer
    {
        private ICarbonGraphics graphics;

        private readonly LimitedList<FrameStatistics> frameStatistics;

        private readonly IDefaultShader defaultShader;
        private readonly IGBufferShader gBufferShader;
        private readonly IDeferredLightShader deferredLightShader;
        private readonly IDebugShader debugShader;
        private readonly IBlendShader blendShader;
        private readonly IShadowMapShader shadowMapShader;

        private DynamicBuffer vertexBuffer;
        private DynamicBuffer indexBuffer;

        private int vertexBufferStride;
        
        private FrameStatistics currentFrameStatistic;

        private Mesh activeMesh;

        private ICarbonShader activeShader;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Renderer(IEngineFactory factory)
        {
            this.frameStatistics = new LimitedList<FrameStatistics>(200);

            this.defaultShader = factory.Get<IDefaultShader>();
            this.gBufferShader = factory.Get<IGBufferShader>();
            this.debugShader = factory.Get<IDebugShader>();
            this.deferredLightShader = factory.Get<IDeferredLightShader>();
            this.blendShader = factory.Get<IBlendShader>();
            this.shadowMapShader = factory.Get<IShadowMapShader>();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.vertexBuffer.Dispose();
            this.indexBuffer.Dispose();

            this.defaultShader.Dispose();
            this.gBufferShader.Dispose();
            this.debugShader.Dispose();
            this.deferredLightShader.Dispose();
            this.blendShader.Dispose();
            this.shadowMapShader.Dispose();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<FrameStatistics> FrameStatistics
        {
            get
            {
                return this.frameStatistics.GetState();
            }
        }

        public override void Initialize(ICarbonGraphics graphic)
        {
            base.Initialize(graphics);

            this.graphics = graphic;

            this.vertexBuffer = graphics.StateManager.GetDynamicBuffer(BindFlags.VertexBuffer);
            this.indexBuffer = graphics.StateManager.GetDynamicBuffer(BindFlags.IndexBuffer);

            this.defaultShader.Initialize(graphics);
            this.gBufferShader.Initialize(graphics);
            this.debugShader.Initialize(graphics);
            this.deferredLightShader.Initialize(graphics);
            this.blendShader.Initialize(graphics);
            this.shadowMapShader.Initialize(graphics);
        }

        public void BeginFrame()
        {
            this.currentFrameStatistic = new FrameStatistics();
            this.graphics.UpdateStates();
        }

        public void AddForwardLighting(IList<RenderLightInstruction> instructions)
        {
            // Todo
            // - Clear all shaders that can support lights of light info
            // - Set the new Lighting information to all shaders that support it

            this.defaultShader.ClearLight();
            foreach (RenderLightInstruction instruction in instructions)
            {
                this.AddForwardLighting(instruction);
            }
        }

        public void AddForwardLighting(RenderLightInstruction instruction)
        {
            switch (instruction.Type)
            {
                case LightType.Ambient:
                    {
                        this.defaultShader.AmbientLight = instruction.Color;
                        break;
                    }

                case LightType.Direction:
                    {
                        this.defaultShader.AddDirectionalLight(instruction.Direction, instruction.Color, instruction.SpecularPower);
                        break;
                    }

                case LightType.Point:
                    {
                        this.defaultShader.AddPointLight(instruction.Position, instruction.Color, instruction.Range, instruction.SpecularPower);
                        break;
                    }

                case LightType.Spot:
                    {
                        this.defaultShader.AddSpotLight(instruction.Position, instruction.Direction, instruction.Color, instruction.Range, instruction.SpotAngles, instruction.SpecularPower);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public void SetDeferredLighting(RenderLightInstruction instruction)
        {
            switch (instruction.Type)
            {
                case LightType.Ambient:
                    {
                        this.deferredLightShader.SetAmbient(instruction.Color);
                        break;
                    }

                case LightType.Direction:
                    {
                        this.deferredLightShader.SetDirectional(instruction.Position, instruction.Direction, instruction.Color);
                        break;
                    }

                case LightType.Point:
                    {
                        this.deferredLightShader.SetPoint(instruction.Position, instruction.Color, instruction.Range, Matrix.Transpose(instruction.View * instruction.Projection));
                        break;
                    }

                case LightType.Spot:
                    {
                        this.deferredLightShader.SetSpot(instruction.Position, instruction.Direction, instruction.Color, instruction.Range, instruction.SpotAngles, Matrix.Transpose(instruction.View * instruction.Projection));
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public void Render(RenderParameters parameters, RenderInstruction instruction)
        {
            this.currentFrameStatistic.Instructions++;

            // Commence the rendering
            this.RenderInstruction(this.graphics.ImmediateContext, parameters, instruction);

            // Do Post-processing here
            // <--
        }

        public void EndFrame()
        {
            // Present everything
            this.graphics.Present(PresentFlags.None);
            
            // End and store the frame statistic
            this.currentFrameStatistic.EndFrame();
            this.frameStatistics.Add(this.currentFrameStatistic);
        }

        public void ClearCache()
        {
            this.defaultShader.ForceReloadOnNextPass = true;
            this.gBufferShader.ForceReloadOnNextPass = true;
            this.deferredLightShader.ForceReloadOnNextPass = true;
            this.debugShader.ForceReloadOnNextPass = true;
            this.blendShader.ForceReloadOnNextPass = true;
            this.shadowMapShader.ForceReloadOnNextPass = true;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void RenderInstruction(DeviceContext context, RenderParameters parameters, RenderInstruction instruction)
        {
                // Check if we have to update the mesh data
                if (this.activeMesh != instruction.Mesh)
                {
                    this.activeMesh = instruction.Mesh;
                    this.UploadMesh(this.activeMesh);
                }

                this.ApplyShader(context, parameters, instruction);

                context.InputAssembler.PrimitiveTopology = instruction.Topology;

                // Render the instruction
                if (instruction.InstanceCount > 1)
                {
                    if (instruction.Mesh.IndexCount > 0)
                    {
                        this.RenderIndexedInstanced(context, instruction);
                    }
                    else
                    {
                        this.RenderInstanced(context, instruction);
                    }
                }
                else
                {
                    this.RenderIndexed(context);
                }
        }

        private void ApplyShader(DeviceContext context, RenderParameters parameters, RenderInstruction instruction)
        {
            // Prepare the Shader
            switch (parameters.Mode)
            {
                case RenderMode.Default:
                    {
                        this.defaultShader.LightingEnabled = parameters.LightingEnabled;
                        this.ActivateShader(context, this.defaultShader);
                        this.defaultShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.GBuffer:
                    {
                        this.ActivateShader(context, this.gBufferShader);
                        this.gBufferShader.Apply(context, typeof(PositionNormalTangentVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.Light:
                    {
                        this.ActivateShader(context, this.deferredLightShader);
                        this.deferredLightShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.Normal:
                    {
                        this.debugShader.Mode = DebugShaderMode.Normal;
                        this.ActivateShader(context, this.debugShader);
                        this.debugShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.Depth:
                    {
                        this.debugShader.Mode = DebugShaderMode.Depth;
                        this.ActivateShader(context, this.debugShader);
                        this.debugShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.Blend:
                    {
                        this.ActivateShader(context, this.blendShader);
                        this.blendShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        break;
                    }

                case RenderMode.ShadowMap:
                    {
                        this.ActivateShader(context, this.shadowMapShader);
                        this.shadowMapShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException("RenderMode not implemented: " + parameters.Mode);
                    }
            }   
        }

        private void ActivateShader(DeviceContext context, ICarbonShader shader)
        {
            if (this.activeShader != shader)
            {
                this.activeShader = shader;
                this.activeShader.ResetConfigurationState(context);
            }
        }

        private void RenderIndexedInstanced(DeviceContext context, RenderInstruction instruction)
        {
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer.Buffer, this.vertexBufferStride, 0));
            context.InputAssembler.SetIndexBuffer(this.indexBuffer.Buffer, Format.R32_UInt, 0);

            context.DrawIndexedInstanced(instruction.Mesh.IndexCount, instruction.InstanceCount, 0, 0, 0);
            this.currentFrameStatistic.InstanceCount += (ulong)instruction.InstanceCount;
            this.currentFrameStatistic.DrawIndexedInstancedCalls++;
            this.currentFrameStatistic.Triangles += ((uint)(instruction.Mesh.IndexCount / 3)) * (ulong)instruction.InstanceCount;
        }

        private void RenderInstanced(DeviceContext context, RenderInstruction instruction)
        {
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer.Buffer, this.vertexBufferStride, 0));

            context.DrawInstanced(instruction.Mesh.ElementCount, instruction.InstanceCount, 0, 0);
            this.currentFrameStatistic.InstanceCount += (ulong)instruction.InstanceCount;
            this.currentFrameStatistic.DrawInstancedCalls++;
            this.currentFrameStatistic.Triangles += ((uint)(instruction.Mesh.ElementCount)) * (ulong)instruction.InstanceCount;
        }

        private void RenderIndexed(DeviceContext context)
        {
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer.Buffer, this.vertexBufferStride, 0));
            context.InputAssembler.SetIndexBuffer(this.indexBuffer.Buffer, Format.R32_UInt, 0);

            context.DrawIndexed(activeMesh.IndexCount, 0, 0);
            this.currentFrameStatistic.DrawIndexedCalls++;
            this.currentFrameStatistic.Triangles += (uint)(activeMesh.IndexCount / 3);
        }

        private void UploadMesh(Mesh mesh)
        {
            this.currentFrameStatistic.MeshSwitches++;
            this.vertexBufferStride = PositionNormalTangentVertex.Size;

            DataStream stream;
            this.vertexBuffer.BeginUpdate(out stream, mesh.GetSizeAs<PositionNormalTangentVertex>());
            mesh.WriteData<PositionNormalTangentVertex>(stream);
            this.vertexBuffer.EndUpdate();

            if (mesh.IndexCount > 0)
            {
                this.indexBuffer.BeginUpdate(out stream, mesh.IndexSize);
                mesh.WriteIndexData(stream);
                this.indexBuffer.EndUpdate();
            }
        }
    }
}
