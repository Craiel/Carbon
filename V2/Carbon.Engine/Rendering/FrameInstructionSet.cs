using System.Collections.Generic;

using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Resource;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Rendering.RenderTarget;

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
        }

        public ICamera Camera { get; private set; }
        public RenderTargetDescription DesiredTarget { get; set; }

        public bool DepthEnabled { get; set; }
        public bool LightingEnabled { get; set; }

        public FrameTechnique Technique { get; set; }

        public List<FrameInstruction> Instructions { get; set; }
        public List<LightInstruction> LightInstructions { get; set; }
    }
}
