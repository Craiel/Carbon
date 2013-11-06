namespace Core.Engine.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;
    using Core.Engine.Rendering.Shaders;

    using Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;

    public class Renderer : EngineComponent, IRenderer
    {
        private readonly LimitedList<FrameStatistics> frameStatistics;

        private readonly IForwardShader forwardShader;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly IGBufferShader gBufferShader;
        private readonly IDeferredLightShader deferredLightShader;
        private readonly IDebugShader debugShader;
        private readonly IBlendShader blendShader;
        private readonly IShadowMapShader shadowMapShader;
        private readonly IPlainShader plainShader;

        private ICarbonGraphics graphics;

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

            this.forwardShader = factory.Get<IForwardShader>();
            this.gBufferShader = factory.Get<IGBufferShader>();
            this.debugShader = factory.Get<IDebugShader>();
            this.deferredLightShader = factory.Get<IDeferredLightShader>();
            this.blendShader = factory.Get<IBlendShader>();
            this.shadowMapShader = factory.Get<IShadowMapShader>();
            this.plainShader = factory.Get<IPlainShader>();
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
            base.Initialize(graphic);

            this.graphics = graphic;

            this.vertexBuffer = graphic.StateManager.GetDynamicBuffer(BindFlags.VertexBuffer);
            this.indexBuffer = graphic.StateManager.GetDynamicBuffer(BindFlags.IndexBuffer);

            this.forwardShader.Initialize(graphic);
            this.gBufferShader.Initialize(graphic);
            this.debugShader.Initialize(graphic);
            this.deferredLightShader.Initialize(graphic);
            this.blendShader.Initialize(graphic);
            this.shadowMapShader.Initialize(graphic);
            this.plainShader.Initialize(graphic);
        }

        public void BeginFrame()
        {
            this.currentFrameStatistic = new FrameStatistics();
        }

        public void AddForwardLighting(IList<RenderLightInstruction> instructions)
        {
            this.forwardShader.ClearLight();
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
                        this.forwardShader.AmbientLight = instruction.Color;
                        break;
                    }

                case LightType.Direction:
                    {
                        this.forwardShader.AddDirectionalLight(instruction.Direction, instruction.Color, instruction.SpecularPower);
                        break;
                    }

                case LightType.Point:
                    {
                        this.forwardShader.AddPointLight(instruction.Position, instruction.Color, instruction.Range, instruction.SpecularPower);
                        break;
                    }

                case LightType.Spot:
                    {
                        this.forwardShader.AddSpotLight(instruction.Position, instruction.Direction, instruction.Color, instruction.Range, instruction.SpotAngles, instruction.SpecularPower);
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
                        this.deferredLightShader.SetSpot(instruction.Position, instruction.Direction, instruction.Color, instruction.Range, instruction.SpotAngles, instruction.IsCastingShadow, Matrix.Transpose(instruction.View * instruction.Projection));
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

            // Set the state according to the instruction
            this.graphics.IsDepthEnabled = parameters.DepthEnabled;
            this.graphics.FillMode = parameters.RenderSolid ? FillMode.Solid : FillMode.Wireframe;
            this.graphics.CullMode = parameters.CullMode;
            this.graphics.UpdateStates();

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
            this.forwardShader.ForceReloadOnNextPass = true;
            this.gBufferShader.ForceReloadOnNextPass = true;
            this.deferredLightShader.ForceReloadOnNextPass = true;
            this.debugShader.ForceReloadOnNextPass = true;
            this.blendShader.ForceReloadOnNextPass = true;
            this.shadowMapShader.ForceReloadOnNextPass = true;
            this.plainShader.ForceReloadOnNextPass = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.vertexBuffer.Dispose();
            this.indexBuffer.Dispose();

            this.forwardShader.Dispose();
            this.gBufferShader.Dispose();
            this.debugShader.Dispose();
            this.deferredLightShader.Dispose();
            this.blendShader.Dispose();
            this.shadowMapShader.Dispose();
            this.plainShader.Dispose();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void RenderInstruction(DeviceContext context, RenderParameters parameters, RenderInstruction instruction)
        {
            Type neededVertexBufferType = this.ApplyShader(context, parameters, instruction);

            // Check if we have to update the mesh data
            if (this.activeMesh != instruction.Mesh)
            {
                this.activeMesh = instruction.Mesh;
                this.UploadMesh(neededVertexBufferType, this.activeMesh);
            }

            context.InputAssembler.PrimitiveTopology = parameters.Topology;

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
            else if (this.activeMesh.IndexCount > 0)
            {
                this.RenderIndexed(context);
            }
            else
            {
                this.Render(context);
            }
        }

        private Type ApplyShader(DeviceContext context, RenderParameters parameters, RenderInstruction instruction)
        {
            // Prepare the Shader
            switch (parameters.Mode)
            {
                case RenderMode.Default:
                    {
                        this.forwardShader.LightingEnabled = parameters.LightingEnabled;
                        this.ActivateShader(context, this.forwardShader);
                        this.forwardShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        return typeof(PositionNormalTangentVertex);
                    }

                case RenderMode.GBuffer:
                    {
                        this.ActivateShader(context, this.gBufferShader);
                        this.gBufferShader.Apply(context, typeof(PositionNormalTangentVertex), parameters, instruction);
                        return typeof(PositionNormalTangentVertex);
                    }

                case RenderMode.Light:
                    {
                        this.ActivateShader(context, this.deferredLightShader);
                        this.deferredLightShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        return typeof(PositionNormalTangentVertex);
                    }

                case RenderMode.Normal:
                    {
                        this.debugShader.Mode = DebugShaderMode.Normal;
                        this.ActivateShader(context, this.debugShader);
                        this.debugShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        return typeof(PositionNormalVertex);
                    }

                case RenderMode.Depth:
                    {
                        this.debugShader.Mode = DebugShaderMode.Depth;
                        this.ActivateShader(context, this.debugShader);
                        this.debugShader.Apply(context, typeof(PositionNormalVertex), parameters, instruction);
                        return typeof(PositionVertex);
                    }

                case RenderMode.Blend:
                    {
                        this.ActivateShader(context, this.blendShader);
                        this.blendShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        return typeof(PositionNormalTangentVertex);
                    }

                case RenderMode.ShadowMap:
                    {
                        this.ActivateShader(context, this.shadowMapShader);
                        this.shadowMapShader.Apply(context, typeof(PositionVertex), parameters, instruction);
                        return typeof(PositionVertex);
                    }

                case RenderMode.Plain:
                    {
                        this.ActivateShader(context, this.plainShader);
                        this.plainShader.Apply(context, typeof(PositionColorVertex), parameters, instruction);
                        return typeof(PositionColorVertex);
                    }

                default:
                    {
                        throw new DataException("RenderMode not implemented: " + parameters.Mode);
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
            this.currentFrameStatistic.Triangles += ((uint)instruction.Mesh.ElementCount) * (ulong)instruction.InstanceCount;
        }

        private void RenderIndexed(DeviceContext context)
        {
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer.Buffer, this.vertexBufferStride, 0));
            context.InputAssembler.SetIndexBuffer(this.indexBuffer.Buffer, Format.R32_UInt, 0);

            context.DrawIndexed(this.activeMesh.IndexCount, 0, 0);
            this.currentFrameStatistic.DrawIndexedCalls++;
            this.currentFrameStatistic.Triangles += (uint)(this.activeMesh.IndexCount / 3);
        }

        private void Render(DeviceContext context)
        {
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer.Buffer, this.vertexBufferStride, 0));

            context.Draw(this.activeMesh.ElementCount, 0);
            this.currentFrameStatistic.DrawCalls++;
            this.currentFrameStatistic.Triangles += (uint)this.activeMesh.ElementCount;
        }

        private void UploadMesh(Type neededVertexBufferType, Mesh mesh)
        {
            this.currentFrameStatistic.MeshSwitches++;
            this.vertexBufferStride = InputStructures.InputLayoutSizes[neededVertexBufferType];

            DataStream stream;
            this.vertexBuffer.BeginUpdate(out stream, mesh.GetSizeAs(neededVertexBufferType));
            mesh.WriteData(neededVertexBufferType, stream);
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
