using System.Collections.Generic;

using Core.Engine.Contracts.Rendering;
using Core.Engine.Resource;

using SlimDX;

namespace Core.Engine.Rendering
{
    using Core.Engine.Rendering.RenderTarget;

    using SlimDX.Direct3D11;

    public struct FrameInstruction
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        public Matrix World { get; set; }
    }

    public enum FrameTechnique
    {
        Forward,
        Deferred,
        Plain,

        DebugNormal,
        DebugDepth,
    }

    public sealed class FrameInstructionSet
    {
        public FrameInstructionSet(ICamera camera)
        {
            this.Camera = camera;
            this.Instructions = new List<FrameInstruction>();
            this.LightInstructions = new List<LightInstruction>();
            this.DesiredTarget = new RenderTargetDescription { Type = RenderTargetType.Default };
            this.Technique = FrameTechnique.Forward;
            this.LightingEnabled = true;
            this.DepthEnabled = true;
            this.RenderSolid = true;
            this.CullMode = CullMode.Back;
            this.Topology = PrimitiveTopology.TriangleList;
        }

        public ICamera Camera { get; private set; }
        public RenderTargetDescription DesiredTarget { get; set; }

        public bool DepthEnabled { get; set; }
        public bool LightingEnabled { get; set; }
        public bool RenderSolid { get; set; }
        public CullMode CullMode { get; set; }
        public PrimitiveTopology Topology { get; set; }

        public FrameTechnique Technique { get; set; }

        public List<FrameInstruction> Instructions { get; set; }
        public List<LightInstruction> LightInstructions { get; set; }
    }
}
