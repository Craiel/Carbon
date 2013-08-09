namespace Core.Engine.Logic
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Engine.Contracts.Logic;

    public abstract class BoundController : EngineComponent, IBoundController
    {
        private readonly IInputManager inputManager;

        private readonly IList<string> modifiers;
        private readonly List<InputBindingEntry> bindingTriggerCache;

        private InputBindings bindings;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BoundController(IInputManager inputManager)
        {
            this.inputManager = inputManager;
            this.inputManager.RegisterReceiver(this);
            
            this.modifiers = new List<string>();
            this.bindingTriggerCache = new List<InputBindingEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; set; }
        
        public virtual void ReceivePersists(string input, object argument = null)
        {
            if (!this.IsActive)
            {
                return;
            }

            switch (input)
            {
                default:
                    {
                        this.bindingTriggerCache.Clear();
                        var activeBindings = this.bindings.GetBindings(input);
                        if (activeBindings != null && activeBindings.Length > 0)
                        {
                            foreach (InputBindingEntry binding in activeBindings)
                            {
                                if (binding.TriggerMode != InputBindingTriggerMode.Always
                                    && binding.TriggerMode != InputBindingTriggerMode.HeldOnly)
                                {
                                    continue;
                                }

                                if (this.MatchModifiers(binding))
                                {
                                    this.bindingTriggerCache.Add(binding);
                                }
                            }
                        }

                        if (this.bindingTriggerCache.Count > 0)
                        {
                            this.OnBindingsTriggeredPersist(this.bindingTriggerCache.AsReadOnly());
                        }

                        break;
                    }
            }
        }

        public virtual void ReceivePressed(string input, object argument = null)
        {
            if (!this.IsActive)
            {
                return;
            }

            if (this.bindings.UsedModifiers.Contains(input))
            {
                this.modifiers.Add(input);
            }

            switch (input)
            {
                default:
                    {
                        this.bindingTriggerCache.Clear();
                        var activeBindings = this.bindings.GetBindings(input);
                        if (activeBindings != null && activeBindings.Length > 0)
                        {
                            foreach (InputBindingEntry binding in activeBindings)
                            {
                                if (binding.TriggerMode != InputBindingTriggerMode.Always
                                    && binding.TriggerMode != InputBindingTriggerMode.Press
                                    && binding.TriggerMode != InputBindingTriggerMode.PressAndRelease)
                                {
                                    continue;
                                }

                                if (this.MatchModifiers(binding))
                                {
                                    this.bindingTriggerCache.Add(binding);
                                }
                            }
                        }

                        if (this.bindingTriggerCache.Count > 0)
                        {
                            this.OnBindingsTriggered(this.bindingTriggerCache.AsReadOnly());
                        }

                        break;
                    }
            }
        }

        public virtual void ReceiveReleased(string input, object argument = null)
        {
            if (!this.IsActive)
            {
                return;
            }

            if (this.modifiers.Contains(input))
            {
                this.modifiers.Remove(input);
            }

            switch (input)
            {
                default:
                    {
                        this.bindingTriggerCache.Clear();
                        var activeBindings = this.bindings.GetBindings(input);
                        if (activeBindings != null && activeBindings.Length > 0)
                        {
                            foreach (InputBindingEntry binding in activeBindings)
                            {
                                if (binding.TriggerMode != InputBindingTriggerMode.Always
                                    && binding.TriggerMode != InputBindingTriggerMode.Release
                                    && binding.TriggerMode != InputBindingTriggerMode.PressAndRelease)
                                {
                                    continue;
                                }

                                if (this.MatchModifiers(binding))
                                {
                                    this.bindingTriggerCache.Add(binding);
                                }
                            }
                        }

                        if (this.bindingTriggerCache.Count > 0)
                        {
                            this.OnBindingsTriggeredRelease(this.bindingTriggerCache.AsReadOnly());
                        }

                        break;
                    }
            }
        }

        public virtual void ReceiveAxisChange(string axis, float value)
        {
        }
        
        public override void Dispose()
        {
            this.inputManager.UnregisterReceiver(this);

            base.Dispose();
        }

        public void SetInputBindings(string name)
        {
            this.bindings = this.inputManager.GetBindings(name);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void OnBindingsTriggered(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
        }

        protected virtual void OnBindingsTriggeredRelease(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
        }

        protected virtual void OnBindingsTriggeredPersist(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool MatchModifiers(InputBindingEntry entry)
        {
            if (entry.Modifiers == null)
            {
                if (this.modifiers.Count <= 0)
                {
                    return true;
                }

                return false;
            }

            if (entry.ModifierMode == InputBindingModifierMode.And)
            {
                return entry.Modifiers.SequenceEqual(this.modifiers);
            }

            return this.modifiers.Any(x => entry.Modifiers.Contains(x));
        }
    }
}
