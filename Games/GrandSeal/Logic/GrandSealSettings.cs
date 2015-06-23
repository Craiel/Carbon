namespace GrandSeal.Logic
{
    using System;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    using Contracts;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    public class GrandSealSettings : EngineComponent, IGrandSealSettings
    {
        public const string BindingSystemController = "systemController";

        private readonly IFactory factory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GrandSealSettings(IFactory factory)
        {
            this.factory = factory;

            this.SetupDefaultBindings();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action SettingsChanged;

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.Reload();
        }

        public void Reload()
        {
            System.Diagnostics.Trace.TraceError("Reload is not implemented");
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SetupDefaultBindings()
        {
            var inputManager = this.factory.Resolve<IInputManager>();

            var binding = inputManager.RegisterBinding(BindingSystemController);
            binding.BindEx("F3", "ToggleDebugOverlay", "PressAndRelease", "And");
            binding.BindEx("F4", "ToggleDebugCamera", "PressAndRelease", "And");
            binding.BindEx("F5", "ToggleDebugDisplay", "PressAndRelease", "And");
            binding.BindEx("F9", "ToggleDepth", "PressAndRelease", "And");
            binding.BindEx("F10", "ToggleWireframe", "PressAndRelease", "And");
        }
    }
}
