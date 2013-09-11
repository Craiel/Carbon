using System;
using System.Collections.Generic;

namespace GrandSeal.Logic
{
    using Contracts;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    public enum GrandSealSystemAction
    {
        ToggleDebugOverlay,
        ToggleDebugCamera,
        ToggleDepth,
        ToggleWireframe,
    }

    public class GrandSealSystemController : BoundController, IGrandSealSystemController
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GrandSealSystemController(IInputManager inputManager)
            : base(inputManager)
        {
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<GrandSealSystemAction> ActionTriggered;

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.SetInputBindings(GrandSealSettings.BindingSystemController);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnBindingsTriggered(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            // If we have no one listening we dont need to care
            if (this.ActionTriggered == null)
            {
                return;
            }

            foreach (InputBindingEntry binding in triggeredBindings)
            {
                GrandSealSystemAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    this.ActionTriggered(action);
                }
            }
        }
    }
}
