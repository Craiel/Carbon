using System.Collections.Generic;
using System.Linq;

using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public interface IBoundController : IEngineComponent, IKeyStateReceiver
    {
        bool IsActive { get; set; }

        void SetInputBindings(string name);
    }

    public abstract class BoundController : EngineComponent, IBoundController
    {
        private readonly IInputManager inputManager;

        private readonly IList<Key> modifiers;
        private readonly List<InputBindingEntry> bindingTriggerCache;

        private InputBindings bindings;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BoundController(IInputManager inputManager)
        {
            this.inputManager = inputManager;
            this.inputManager.RegisterReceiver(this);
            
            this.modifiers = new List<Key>();
            this.bindingTriggerCache = new List<InputBindingEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; set; }

        public virtual void ReceivePersists(Key key)
        {
        }
        
        public virtual void ReceivePressed(Key key)
        {
            if (!this.IsActive)
            {
                return;
            }

            if (this.bindings.UsedModifiers.Contains(key))
            {
                this.modifiers.Add(key);
            }

            switch (key)
            {
                default:
                    {
                        this.bindingTriggerCache.Clear();
                        var activeBindings = this.bindings.GetBindings(key);
                        if (activeBindings != null && activeBindings.Length > 0)
                        {
                            foreach (InputBindingEntry binding in activeBindings)
                            {
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

        public virtual void ReceiveReleased(Key key)
        {
            if (!this.IsActive)
            {
                return;
            }

            if (this.modifiers.Contains(key))
            {
                this.modifiers.Remove(key);
            }
        }

        public override void Update(ITimer gameTime)
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
