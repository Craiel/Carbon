using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Rendering;

namespace Core.Engine.Logic
{
    using System;

    using SlimDX.Direct3D11;

    public interface IDebugController : IBoundController
    {
    }

    public class DebugController : BoundController, IDebugController
    {
        internal enum DebugControllerAction
        {
            ToggleDebugOverlay,
            ToggleDepth,
            ToggleWireframe,
        }

        private readonly ICarbonGraphics graphics;
        private readonly IFrameManager frameManager;

        private bool depthState;
        private bool wireframeState;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DebugController(IInputManager inputManager, ICarbonGraphics graphics, IFrameManager frameManager)
            : base(inputManager)
        {
            this.frameManager = frameManager;

            this.graphics = graphics;

            this.SetInputBindings("debug");
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnBindingsTriggered(System.Collections.Generic.IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (InputBindingEntry binding in triggeredBindings)
            {
                DebugControllerAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    this.OnAction(action);
                }
            }

            this.UpdateStates();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnAction(DebugControllerAction action)
        {
            switch (action)
            {
                case DebugControllerAction.ToggleDepth:
                    {
                        this.depthState = !this.depthState;
                        break;
                    }

                case DebugControllerAction.ToggleWireframe:
                    {
                        this.wireframeState = !this.wireframeState;
                        break;
                    }

                case DebugControllerAction.ToggleDebugOverlay:
                    {
                        this.frameManager.EnableDebugOverlay = !this.frameManager.EnableDebugOverlay;
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException("No action implemented for " + action);
                    }
            }
        }

        private void UpdateStates()
        {
            this.graphics.IsDepthEnabled = this.depthState;
            this.graphics.FillMode = this.wireframeState ? FillMode.Wireframe : FillMode.Solid;
        }
    }
}
