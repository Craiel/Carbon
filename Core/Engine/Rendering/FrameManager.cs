namespace Core.Engine.Rendering
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Contracts.IoC;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;
    using Core.Engine.Rendering.Primitives;
    using Core.Engine.Rendering.RenderTarget;
    using SharpDX;
    using SharpDX.Direct3D11;

    public class FrameManager : EngineComponent, IFrameManager
    {
        private readonly ICarbonGraphics graphics;
        private readonly IRenderer renderer;
        
        private readonly IList<RenderInstruction> instructionCache;
        private readonly IList<RenderLightInstruction> lightInstructionCache;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly GBufferRenderTarget gBufferTarget;
        private readonly BackBufferRenderTarget backBufferRenderTarget;
        private readonly TextureRenderTarget deferredLightTarget;
        private readonly DepthRenderTarget shadowMapTarget;
        private readonly IDictionary<int, TextureRenderTarget> textureTargets;
        private readonly IDictionary<int, TextureData> shadowMapCache;
        
        private bool targetTexturesRegistered;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FrameManager(IFactory factory)
        {
            this.graphics = factory.Resolve<ICarbonGraphics>();
            this.renderer = factory.Resolve<IRenderer>();

            this.instructionCache = new List<RenderInstruction>();
            this.lightInstructionCache = new List<RenderLightInstruction>();

            this.backBufferRenderTarget = new BackBufferRenderTarget();
            this.gBufferTarget = new GBufferRenderTarget();
            this.deferredLightTarget = new TextureRenderTarget { BlendMode = RendertargetBlendMode.Additive };
            this.shadowMapTarget = new DepthRenderTarget();
            this.textureTargets = new Dictionary<int, TextureRenderTarget>();
            this.shadowMapCache = new Dictionary<int, TextureData>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector4 BackgroundColor { get; set; }

        public void BeginFrame()
        {
            this.backBufferRenderTarget.Clear(this.graphics, this.BackgroundColor);
            this.gBufferTarget.Clear(this.graphics, this.BackgroundColor);
            this.deferredLightTarget.Clear(this.graphics, Vector4.Zero);
        }

        public void Resize(TypedVector2<int> size)
        {
            if (this.targetTexturesRegistered)
            {
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.GBufferNormal);
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.GBufferDiffuse);
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.GBufferSpecular);
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.GBufferDepth);
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.DeferredLight);
                this.graphics.TextureManager.Unregister((int)StaticTextureRegister.ShadowMapTarget);
            }

            this.backBufferRenderTarget.Resize(this.graphics, size);
            this.gBufferTarget.Resize(this.graphics, size);
            this.deferredLightTarget.Resize(this.graphics, size);

            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.NormalData, (int)StaticTextureRegister.GBufferNormal, size);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.DiffuseData, (int)StaticTextureRegister.GBufferDiffuse, size);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.SpecularData, (int)StaticTextureRegister.GBufferSpecular, size);
            this.graphics.TextureManager.RegisterStatic(this.gBufferTarget.DepthData, (int)StaticTextureRegister.GBufferDepth, size);
            this.graphics.TextureManager.RegisterStatic(this.deferredLightTarget.Data, (int)StaticTextureRegister.DeferredLight, size);
            this.graphics.TextureManager.RegisterStatic(this.shadowMapTarget.Data, (int)StaticTextureRegister.ShadowMapTarget, size);
            
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

                case FrameTechnique.Plain:
                    {
                        this.RenderSetPlain(set, RenderMode.Plain);
                        break;
                    }
            }
        }

        public void ClearCache()
        {
            foreach (int key in this.shadowMapCache.Keys)
            {
                this.shadowMapCache[key].Dispose();
            }

            this.shadowMapCache.Clear();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.backBufferRenderTarget.Dispose();
            this.gBufferTarget.Dispose();
            this.deferredLightTarget.Dispose();
            this.shadowMapTarget.Dispose();

            foreach (TextureRenderTarget target in this.textureTargets.Values)
            {
                target.Dispose();
            }

            this.textureTargets.Clear();

            this.ClearCache();

            base.Dispose(true);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateShadowMap(RenderParameters parameters, RenderLightInstruction instruction, IList<RenderInstruction> instructions)
        {
            int key = instruction.GetShadowMapKey();
            if (this.shadowMapCache.ContainsKey(key))
            {
                if (instruction.RegenerateShadowMap)
                {
                    this.shadowMapCache[key].Dispose();
                    this.shadowMapCache.Remove(key);
                }
                else
                {
                    return;
                }
            }

            var lightCameraParameters = new RenderParameters
                {
                    DepthEnabled = true,
                    RenderSolid = parameters.RenderSolid,
                    CullMode = CullMode.Front,
                    Topology = parameters.Topology,
                    CameraPosition = instruction.Position,
                    LightingEnabled = false,
                    Mode = RenderMode.ShadowMap,
                    Projection = instruction.Projection,
                    View = instruction.View
                };

            // Setup the target with different quality according to the instruction
            this.shadowMapTarget.Resize(this.graphics, new TypedVector2<int>(instruction.ShadowMapSize));
            this.shadowMapTarget.Clear(this.graphics, Vector4.Zero);
            this.shadowMapTarget.Set(this.graphics);

            for (int i = 0; i < instructions.Count; i++)
            {
                this.renderer.Render(lightCameraParameters, instructions[i]);
            }
            
            using (var stream = new MemoryStream())
            {
                this.shadowMapTarget.StoreData(this.graphics.ImmediateContext, stream);
                stream.Position = 0;
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                this.shadowMapCache.Add(key, new TextureData(TextureDataType.Texture2D, data, this.shadowMapTarget.LoadInformation, this.shadowMapTarget.ViewDescription));
            }
        }

        private void RenderSetDeferred(FrameInstructionSet set)
        {
            // Set the state and prepare the target
            this.gBufferTarget.Set(this.graphics);

            // Set the parameters for the rendering process (these don't change regardless of instruction)
            var parameters = new RenderParameters
            {
                DepthEnabled = set.DepthEnabled,
                RenderSolid = set.RenderSolid,
                CullMode = set.CullMode,
                Topology = set.Topology,
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
                if (this.lightInstructionCache[i].Type != LightType.Spot || !this.lightInstructionCache[i].IsCastingShadow)
            {
                    continue;
                }

                this.UpdateShadowMap(parameters, this.lightInstructionCache[i], solidInstructions);
            }

            // get a quad to use for our operations
            var quad = new Mesh(Quad.CreateScreen(new Vector2(-1), new TypedVector2<int>(1)));
            
            // Render the Lighting
            if (set.LightingEnabled && this.lightInstructionCache.Count > 0)
            {
                // Turn off depth for the following parts, we don't want it
                parameters.DepthEnabled = false;
                
                this.deferredLightTarget.Set(this.graphics);
                parameters.Mode = RenderMode.Light;

                for (int i = 0; i < this.lightInstructionCache.Count; i++)
                {
                    var instruction = new RenderInstruction
                                          {
                                              DiffuseTexture = this.gBufferTarget.DiffuseData,
                                              NormalTexture = this.gBufferTarget.NormalData,
                                              SpecularTexture = this.gBufferTarget.SpecularData,
                                              DepthMap = this.gBufferTarget.DepthData,
                                              Mesh = quad
                                          };

                    if (this.lightInstructionCache[i].IsCastingShadow)
                    {
                        int shadowMapKey = this.lightInstructionCache[i].GetShadowMapKey();
                        if (this.shadowMapCache.ContainsKey(shadowMapKey))
                        {
                            instruction.ShadowMap = this.shadowMapCache[shadowMapKey];
                        }
                    }

                    this.renderer.SetDeferredLighting(this.lightInstructionCache[i]);
                    this.renderer.Render(parameters, instruction);
                }
            }
            
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
                            DiffuseTexture = this.gBufferTarget.DiffuseData,
                            SpecularTexture = this.deferredLightTarget.Data,
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
                DepthEnabled = set.DepthEnabled,
                RenderSolid = set.RenderSolid,
                CullMode = set.CullMode,
                Topology = set.Topology,
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
                DepthEnabled = set.DepthEnabled,
                RenderSolid = set.RenderSolid,
                CullMode = set.CullMode,
                Topology = set.Topology,
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

        private void RenderSetPlain(FrameInstructionSet set, RenderMode mode)
        {
            // Set the state and prepare the target
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
                DepthEnabled = set.DepthEnabled,
                RenderSolid = set.RenderSolid,
                CullMode = set.CullMode,
                Topology = set.Topology,
                View = set.Camera.View,
                Projection = set.Camera.Projection,
                Mode = mode,
                LightingEnabled = false
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
                    }
                    else
                    {
                        instructions.Add(this.CreateRenderInstruction(currentInstruction));
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
            var instruction = new RenderInstruction { Mesh = source.Mesh, World = source.World };

            if (source.Material != null)
            {
                instruction.Color = source.Material.ColorDiffuse;

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
                // Set a material to indicate this is has an error
                instruction.Color = new Vector4(1, 0, 0, 1);
            }

            return instruction;
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
            target.Resize(this.graphics, description.Size);
            target.Clear(this.graphics, new Vector4(1));
            target.Set(this.graphics);

            if (needRegister)
            {
                // Register the texture in the texture manager static register
                this.graphics.TextureManager.RegisterStatic(target.Data, description.Index, description.Size);
            }

            return target;
        }
        
        private void ProcessLightInstructions(IList<LightInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                LightInstruction instruction = instructions[i];
                if (instruction.Light == null)
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

                switch (instruction.Light.Type)
                {
                    case LightType.Ambient:
                        {
                            break;
                        }

                    case LightType.Direction:
                        {
                            renderInstruction.Direction = instruction.Light.Direction;
                            renderInstruction.View = instruction.Light.View;
                            renderInstruction.Projection = instruction.Light.Projection;
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
                            renderInstruction.View = instruction.Light.View;
                            renderInstruction.Projection = instruction.Light.Projection;

                            // Shadow parameters
                            renderInstruction.IsCastingShadow = instruction.Light.IsCastingShadow;
                            renderInstruction.RegenerateShadowMap = instruction.Light.NeedShadowUpdate;
                            renderInstruction.ShadowMapSize = 512; // Todo: find better place for this and adjust quality to light parameters

                            // Hack for lack of better judgement...
                            instruction.Light.NeedShadowUpdate = false;
                            break;
                        }

                    default:
                        {
                            throw new DataException("Light type not implemented: " + instruction.Light.Type);
                        }
                }

                this.lightInstructionCache.Add(renderInstruction);
            }
        }
    }
}
