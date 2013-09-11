namespace GrandSeal.Logic
{
    using System;

    using Contracts;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;
    using Core.Utils.Contracts;

    public class GrandSealSettings : EngineComponent, IGrandSealSettings
    {
        public const string BindingSystemController = "systemController";

        private readonly IEngineFactory factory;
        private readonly ILog log;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GrandSealSettings(IEngineFactory factory)
        {
            this.factory = factory;
            this.log = factory.Get<IGrandSealLog>().AquireContextLog("GrandSealSettings");

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
            this.log.Error("Reload is not implemented");
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SetupDefaultBindings()
        {
            var inputManager = this.factory.Get<IInputManager>();

            var binding = inputManager.RegisterBinding(BindingSystemController);
            binding.BindEx("F3", "ToggleDebugOverlay", "PressAndRelease", "And");
            binding.BindEx("F4", "ToggleDebugCamera", "PressAndRelease", "And");
            binding.BindEx("F9", "ToggleDepth", "PressAndRelease", "And");
            binding.BindEx("F10", "ToggleWireframe", "PressAndRelease", "And");
        }
    }
}
