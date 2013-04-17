using System;
using System.Collections;
using System.Collections.Generic;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering.Debug;
using Carbon.Engine.Rendering.Primitives;
using Carbon.Engine.Rendering.RenderTarget;

using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public class FrameManager : EngineComponent, IFrameManager
    {
        private readonly ICarbonGraphics graphics;
        private readonly IRenderer renderer;
        private readonly IProjectionCamera shadowmapCamera;

        private readonly DebugOverlay debugOverlay;
        
        private readonly IList<RenderInstruction> instructionCache;
        private readonly IList<RenderLightInstruction> lightInstructionCache;

        private readonly BackBufferRenderTarget backBufferRenderTarget;
        private readonly GBufferRenderTarget gBufferTarget;
        private readonly TextureRenderTarget deferredLightTarget;
        private readonly DepthRenderTarget shadowMapTarget;
        private readonly IDictionary<int, TextureRenderTarget> textureTargets;

        private bool targetTexturesRegistered;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FrameManager(IEngineFactory factory)
        {
            this.graphics = factory.Get<ICarbonGraphics>();
            this.renderer = factory.Get<IRenderer>();
            this.shadowmapCamera = factory.Get<IProjectionCamera>();

            this.debugOverlay = new DebugOverlay();
            this.instructionCache = new List<RenderInstruction>();
            this.lightInstructionCache = new List<RenderLightInstruction>();

            this.backBufferRenderTarget = new BackBufferRenderTarget();
            this.gBufferTarget = new GBufferRenderTarget();
            this.deferredLightTarget = new TextureRenderTarget { BlendMode = RendertargetBlendMode.Additive};
            this.shadowMapTarget = new DepthRenderTarget();
            this.textureTargets = new Dictionary<int, TextureRenderTarget>();
        }

        public override void Dispose()
        {
            this.debugOverlay.Dispose();

            this.backBufferRenderTarget.Dispose();
            this.gBufferTarget.Dispose();
            this.deferredLightTarget.Dispose();
            this.shadowMapTarget.Dispose();

            foreach (TextureRenderTarget target in this.textureTargets.Values)
            {
                target.Dispose();
            }

            this.textureTargets.Clear();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool EnableDebugOverlay { get; set; }

        public void BeginFrame()
        {
            this.backBufferRenderTarget.Clear(this.graphics, Vector4.Zero);
            this.gBufferTarget.Clear(this.graphics, Vector4.Zero);
            this.deferredLightTarget.Clear(this.graphics, Vector4.Zero);
            this.shadowMapTarget.Clear(this.graphics, Vector4.Zero);
        }

        public void Resize(int width, int height)
        {
            if (this.targetTexturesRegistered)
            {
                this.graphics.TextureManager.Unregister(11);
                this.graphics.TextureManager.Unregister(12);
                this.graphics.TextureManager.Unregister(13);
                this.graphics.TextureManager.Unregister(14);
                this.graphics.TextureManager.Unregister(15);
                this.graphics.TextureManager.Unregister(16);
            }

            this.backBufferRenderTarget.Resize(this.graphics, width, height);

            this.gBufferTarget.Resize(this.graphics, width, height);
            this.deferredLightTarget.Resize(this.graphics, width, height);
            this.shadowMapTarget.Resize(this.graphics, width, height);

            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.NormalView, 11);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.DiffuseView, 12);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.SpecularView, 13);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.DepthView, 14);
            this.graphics.TextureManager.RegisterStatic(this.deferredLightTarget.View, 15);
            this.graphics.TextureManager.RegisterStatic(this.shadowMapTarget.View, 16);
            
            this.targetTexturesRegistered = true;
        }

        public FrameInstructionSet BeginSet(ICamera camera)
        {
            return new FrameInstructionSet(camera);
        }

        public void RenderSet(FrameInstructionSet set)
        {
            this.instructionCache.Clear();
            this.ProcessInstructions(set.Instructions);

            this.lightInstructionCache.Clear();
            if (set.LightingEnabled && set.LightInstructions.Count > 0)
            {
                this.ProcessLightInstructions(set.LightInstructions);
            }

            switch (set.Technique)
            {
                case FrameTechnique.Forward:
                    {
                        if (set.LightingEnabled && this.lightInstructionCache.Count > 0)
                        {
                            this.renderer.AddForwardLighting(this.lightInstructionCache);
                        }

                        this.RenderSetForward(set);
                        break;
                    }

                case FrameTechnique.Deferred:
                    {
                        this.RenderSetDeferred(set);
                        break;
                    }

                case FrameTechnique.DebugNormal:
                    {
                        this.RenderSetDebug(set, RenderMode.Normal);
                        break;
                    }

                case FrameTechnique.DebugDepth:
                    {
                        this.RenderSetDebug(set, RenderMode.Depth);
                        break;
                    }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateShadowMap(RenderLightInstruction instruction, IList<RenderInstruction> instructions)
        {
            var lightCameraParameters = new RenderParameters
                {
                    CameraPosition = instruction.Position,
                    LightingEnabled = false,
                    Mode = RenderMode.ShadowMap,
                    Projection = instruction.Projection,
                    View = instruction.View
                };

            this.shadowMapTarget.Set(this.graphics);
            for (int i = 0; i < instructions.Count; i++)
            {
                this.renderer.Render(lightCameraParameters, instructions[i]);
            }
        }

        private void RenderSetDeferred(FrameInstructionSet set)
        {
            // Set the state and prepare the target
            this.SetGraphicState(set);
            this.gBufferTarget.Set(this.graphics);

            // Set the parameters for the rendering process (these don't change regardless of instruction)
            var parameters = new RenderParameters
            {
                CameraPosition = set.Camera.Position,
                View = set.Camera.View,
                Projection = set.Camera.Projection,
                Mode = RenderMode.GBuffer,
                LightingEnabled = set.LightingEnabled
            };
            
            // Render the scene into the GBuffer for non-transparent objects
            IList<RenderInstruction> transparentInstructions = new List<RenderInstruction>();
            IList<RenderInstruction> solidInstructions = new List<RenderInstruction>();
            for (int i = 0; i < this.instructionCache.Count; i++)
            {
                if (this.instructionCache[i].AlphaTexture != null)
                {
                    transparentInstructions.Add(this.instructionCache[i]);
                }
                else
                {
                    solidInstructions.Add(this.instructionCache[i]);
                    this.renderer.Render(parameters, this.instructionCache[i]);
                }
            }

            for (int i = 0; i < this.lightInstructionCache.Count; i++)
            {
                if (this.lightInstructionCache[i].Type != LightType.Direction && this.lightInstructionCache[i].Type != LightType.Spot)
                {
                    continue;
                }

                this.UpdateShadowMap(this.lightInstructionCache[i], solidInstructions);
            }

            // get a quad to use for our operations
            Mesh quad = new Mesh(Quad.CreateScreen(new Vector2(-1), new Vector2(1)));
            
            // Render the Lighting
            if (set.LightingEnabled && this.lightInstructionCache.Count > 0)
            {
                // Turn off depth for the following parts, we dont want it
                this.graphics.DisableDepth();
                this.graphics.UpdateStates();
                
                this.deferredLightTarget.Set(this.graphics);
                parameters.Mode = RenderMode.Light;

                for (int i = 0; i < this.lightInstructionCache.Count; i++)
                {
                    this.renderer.SetDeferredLighting(this.lightInstructionCache[i]);
                    this.renderer.Render(
                        parameters,
                        new RenderInstruction
                            {
                                DiffuseTexture = this.gBufferTarget.DiffuseView,
                                NormalTexture = this.gBufferTarget.NormalView,
                                SpecularTexture = this.gBufferTarget.SpecularView,
                                DepthMap = this.gBufferTarget.DepthView,
                                ShadowMap = this.shadowMapTarget.View,
                                Mesh = quad
                            });
                }
            }
            
            // Todo:
            // - Set the Lighting target
            // - change the parameters
            // - Render the lights into the light target
            // - Join the targets together with blending
            // - Render the final result into the backbuffer or texture depending on desired target
            // - Render transparent objects over with forward rendering

            // Compose onto the desired target
            RenderTargetBase compositionTarget;
            if (set.DesiredTarget.Type == RenderTargetType.Texture)
            {
                compositionTarget = this.PrepareTextureTarget(set.DesiredTarget);
            }
            else
            {
                compositionTarget = this.backBufferRenderTarget;
            }

            compositionTarget.BlendMode = RendertargetBlendMode.None;
            compositionTarget.Set(this.graphics);
            parameters.Mode = RenderMode.Blend;
            parameters.LightingEnabled = false;
            
            this.renderer.Render(
                        parameters,
                        new RenderInstruction
                        {
                            DiffuseTexture = this.gBufferTarget.DiffuseView,
                            SpecularTexture = this.deferredLightTarget.View,
                            Mesh = quad
                        });

            // Render transparent components on top
            compositionTarget.BlendMode = RendertargetBlendMode.Alpha;
            compositionTarget.Set(this.graphics);
            parameters.Mode = RenderMode.Default;
            for (int i = 0; i < transparentInstructions.Count; i++)
            {
                this.renderer.Render(parameters, transparentInstructions[i]);
            }

            compositionTarget.BlendMode = RendertargetBlendMode.None;
        }

        private void RenderSetForward(FrameInstructionSet set)
        {
            // Set the state and prepare the target
            this.SetGraphicState(set);

            RenderTargetBase compositionTarget;
            if (set.DesiredTarget.Type == RenderTargetType.Texture)
            {
                compositionTarget = this.PrepareTextureTarget(set.DesiredTarget);
            }
            else
            {
                compositionTarget = this.backBufferRenderTarget;
            }

            compositionTarget.BlendMode = RendertargetBlendMode.Alpha;
            compositionTarget.Set(this.graphics);

            // Set the parameters for the rendering process (these don't change regardless of instruction)
            var parameters = new RenderParameters
            {
                CameraPosition = set.Camera.Position,
                View = set.Camera.View,
                Projection = set.Camera.Projection,
                Mode = RenderMode.Default,
                LightingEnabled = set.LightingEnabled
            };

            for (int i = 0; i < this.instructionCache.Count; i++)
            {
                this.renderer.Render(parameters, this.instructionCache[i]);
            }
        }

        private void RenderSetDebug(FrameInstructionSet set, RenderMode mode)
        {
            // Set the state and prepare the target
            this.SetGraphicState(set);
            if (set.DesiredTarget.Type == RenderTargetType.Texture)
            {
                this.PrepareTextureTarget(set.DesiredTarget);
            }
            else
            {
                this.backBufferRenderTarget.Set(this.graphics);
            }

            // Set the parameters for the rendering process (these don't change regardless of instruction)
            var parameters = new RenderParameters
            {
                View = set.Camera.View,
                Projection = set.Camera.Projection,
                Mode = mode,
                LightingEnabled = set.LightingEnabled
            };

            for (int i = 0; i < this.instructionCache.Count; i++)
            {
                this.renderer.Render(parameters, this.instructionCache[i]);
            }
        }

        private void ProcessInstructions(IList<FrameInstruction> source)
        {
            var sortingTable = new Hashtable();
            for (int i = 0; i < source.Count; i++)
            {
                FrameInstruction currentInstruction = source[i];
                Mesh mesh = currentInstruction.Mesh;
                Material material = currentInstruction.Material;

                if (mesh == null || mesh.ElementCount <= 0)
                {
                    continue;
                }

                int key = HashUtils.CombineObjectHashes(new object[] { mesh, material });

                if (!sortingTable.ContainsKey(key))
                {
                    sortingTable.Add(key, new List<RenderInstruction> { this.CreateRenderInstruction(currentInstruction) });
                }
                else
                {
                    var instructions = (IList<RenderInstruction>)sortingTable[key];
                    if (!mesh.AllowInstancing)
                    {
                        instructions.Add(this.CreateRenderInstruction(currentInstruction));
                        continue;
                    }

                    if (instructions[instructions.Count - 1].InstanceCount < RenderInstruction.MaxInstanceCount)
                    {
                        if (instructions[instructions.Count - 1].InstanceCount == 0)
                        {
                            instructions[instructions.Count - 1].AddInstance(instructions[instructions.Count - 1].World);
                        }

                        instructions[instructions.Count - 1].AddInstance(currentInstruction.World);
                        // this.currentFrameStatistic.InstructionsDiscarded++;
                    }
                    else
                    {
                        instructions.Add(this.CreateRenderInstruction(currentInstruction));
                        // this.currentFrameStatistic.InstanceLimitExceeded++;
                    }
                }
            }

            source.Clear();
            var list = new IList<RenderInstruction>[sortingTable.Values.Count];
            sortingTable.Values.CopyTo(list, 0);
            for (int i = 0; i < list.Length; i++)
            {
                for (int n = 0; n < list[i].Count; n++)
                {
                    this.instructionCache.Add(list[i][n]);
                }
            }
        }

        private RenderInstruction CreateRenderInstruction(FrameInstruction source)
        {
            var instruction = new RenderInstruction { Mesh = source.Mesh, World = source.World};

            if(source.Material != null)
            {
                instruction.Color = source.Material.Color;

                if (source.Material.DiffuseTexture != null)
                {
                    instruction.DiffuseTexture = this.graphics.TextureManager.GetTexture(source.Material.DiffuseTexture);
                }

                if (source.Material.NormalTexture != null)
                {
                    instruction.NormalTexture = this.graphics.TextureManager.GetTexture(source.Material.NormalTexture);
                }

                if (source.Material.SpecularTexture != null)
                {
                    instruction.SpecularTexture = this.graphics.TextureManager.GetTexture(source.Material.SpecularTexture);
                }

                if (source.Material.AlphaTexture != null)
                {
                    instruction.AlphaTexture = this.graphics.TextureManager.GetTexture(source.Material.AlphaTexture);
                }
            }
            else
            {
                // Set a material to indicate this is errornous
                instruction.Color = new Vector4(1, 0, 0, 1);
            }

            return instruction;
        }

        private void SetGraphicState(FrameInstructionSet set)
        {
            if (set.DepthEnabled)
            {
                this.graphics.EnableDepth();
            }
            else
            {
                this.graphics.DisableDepth();
            }

            graphics.UpdateStates();
        }

        private TextureRenderTarget PrepareTextureTarget(RenderTargetDescription description)
        {
            bool needRegister = false;
            if (!this.textureTargets.ContainsKey(description.Index))
            {
                this.textureTargets.Add(description.Index, new TextureRenderTarget());
                needRegister = true;
            }

            var target = this.textureTargets[description.Index];
            target.Resize(this.graphics, description.Width, description.Height);
            target.Clear(this.graphics, new Vector4(1));
            target.Set(this.graphics);

            if (needRegister)
            {
                // Register the texture in the texture manager static register
                this.graphics.TextureManager.RegisterStatic(target.View, description.Index);
            }

            return target;
        }

        private void GetLightViewProjection(LightInstruction instruction, Vector4 position, out Matrix view, out Matrix projection)
        {
            if (instruction.Light.Type != LightType.Direction && instruction.Light.Type != LightType.Spot)
            {
                throw new ArgumentException();
            }

            // Todo: Near / Far for direction is camera max plus extra, for point its the range
            if (instruction.Light.Type == LightType.Direction)
            {
                this.shadowmapCamera.SetPerspective(1024, 768, 0.05f, 400);
            }
            else
            {
                this.shadowmapCamera.SetPerspective(1024, 768, 0.05f, instruction.Light.Range);
            }

            this.shadowmapCamera.Position = position;
            //this.shadowmapCamera.LookAt(new Vector3(0));
            this.shadowmapCamera.LookAt(instruction.Light.Direction);
            this.shadowmapCamera.Update(null);

            view = this.shadowmapCamera.View;
            projection = this.shadowmapCamera.Projection;
        }

        private void ProcessLightInstructions(IList<LightInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                LightInstruction instruction = instructions[i];
                if(instruction.Light == null)
                {
                    throw new InvalidOperationException("Light Instruction without actual light information!");
                }

                var renderInstruction = new RenderLightInstruction
                    {
                        Color = instruction.Light.Color,
                        Type = instruction.Light.Type,
                        Position = instruction.Position,
                        SpecularPower = instruction.Light.SpecularPower
                    };

                Matrix view;
                Matrix projection;
                switch (instruction.Light.Type)
                {
                    case LightType.Ambient:
                        {
                            break;
                        }

                    case LightType.Direction:
                        {
                            float translation = 40;
                            Vector3 position = Vector3.Transform(instruction.Light.Direction.Invert(), Matrix.Translation(new Vector3(translation))).XYZ();
                            renderInstruction.Position = new Vector4(position, 1);
                            renderInstruction.Direction = instruction.Light.Direction;

                            this.GetLightViewProjection(instruction, renderInstruction.Position, out view, out projection);
                            renderInstruction.View = view;
                            renderInstruction.Projection = projection;
                            break;
                        }

                    case LightType.Point:
                        {
                            renderInstruction.Range = instruction.Light.Range;
                            break;
                        }

                    case LightType.Spot:
                        {
                            renderInstruction.Direction = instruction.Light.Direction;
                            renderInstruction.Range = instruction.Light.Range;
                            renderInstruction.SpotAngles = instruction.Light.SpotAngles;

                            this.GetLightViewProjection(instruction, renderInstruction.Position, out view, out projection);
                            renderInstruction.View = view;
                            renderInstruction.Projection = projection;
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException("Light type not implemented: " + instruction.Light.Type);
                        }
                }

                this.lightInstructionCache.Add(renderInstruction);
            }
        }
    }
}
