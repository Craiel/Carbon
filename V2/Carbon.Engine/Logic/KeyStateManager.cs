using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic.Scripting;

using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public class KeyStateManager : EngineComponent, IKeyStateManager
    {
        private static readonly TimeSpan updateCycle = TimeSpan.FromMilliseconds(10);

        private readonly IList<IKeyStateReceiver> receivers;
        private readonly IDictionary<Key, bool> keyPressedState;

        private readonly IDictionary<string, KeyBindings> keyBindings;

        private readonly DirectInput directInput;
        private readonly Keyboard keyboard;

        private IOrderedEnumerable<IKeyStateReceiver> orderedReceivers;

        private TimeSpan lastUpdateTime;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public KeyStateManager()
        {
            this.receivers = new List<IKeyStateReceiver>();
            this.keyPressedState = new Dictionary<Key, bool>();

            this.directInput = new DirectInput();
            this.keyboard = new Keyboard(directInput);
            this.keyboard.Acquire();

            this.keyBindings = new Dictionary<string, KeyBindings>();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.directInput.Dispose();
            this.keyboard.Dispose();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void RegisterReceiver(IKeyStateReceiver receiver)
        {
            if (!this.receivers.Contains(receiver))
            {
                this.receivers.Add(receiver);
                this.orderedReceivers = this.receivers.OrderBy(x => x.Order);
            }
        }

        public void UnregisterReceiver(IKeyStateReceiver receiver)
        {
            if (this.receivers.Contains(receiver))
            {
                this.receivers.Remove(receiver);
                this.orderedReceivers = this.receivers.OrderBy(x => x.Order);
            }
        }

        public override void Update(ITimer gameTimer)
        {
            if ((gameTimer.ElapsedTime - this.lastUpdateTime) < updateCycle)
            {
                return;
            }

            KeyboardState state = this.keyboard.GetCurrentState();
            IList<Key> previouslyPressedKeys = this.keyPressedState.Keys.Where(key => this.keyPressedState[key]).ToList();

            // Now check if any new ones are pressed
            foreach (Key key in state.PressedKeys)
            {
                if (!this.keyPressedState.ContainsKey(key))
                {
                    this.keyPressedState.Add(key, false);
                }

                // Still pressed keys
                if (previouslyPressedKeys.Contains(key))
                {
                    previouslyPressedKeys.Remove(key);
                    this.OnKeyStatePersists(key);
                }

                // New pressed keys
                if (!this.keyPressedState[key])
                {
                    this.keyPressedState[key] = true;
                    this.OnKeystateChange(key);
                }
            }

            // Check for released keys
            foreach (Key key in previouslyPressedKeys)
            {
                this.keyPressedState[key] = false;
                this.OnKeystateChange(key);
            }

            this.lastUpdateTime = gameTimer.ElapsedTime;
        }

        [ScriptingMethod]
        public KeyBindings RegisterKeyBinding(string name)
        {
            if (this.keyBindings.ContainsKey(name))
            {
                throw new InvalidOperationException("Key bindings with the same name are already registered");
            }

            var bindings = new KeyBindings();
            this.keyBindings.Add(name, bindings);
            return bindings;
        }

        public KeyBindings GetBindings(string name)
        {
            if (!this.keyBindings.ContainsKey(name))
            {
                throw new InvalidOperationException("No keybinding found for name " + name);
            }

            return this.keyBindings[name];
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnKeystateChange(Key key)
        {
            if(this.orderedReceivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            bool isPressed = this.keyPressedState[key];

            foreach (IKeyStateReceiver receiver in this.orderedReceivers)
            {
                bool isExclusive = false;
                if (isPressed)
                {
                    receiver.ReceivePressed(key, ref isExclusive);
                }
                else
                {
                    receiver.ReceiveReleased(key, ref isExclusive);
                }

                if (isExclusive)
                {
                    break;
                }
            }
        }

        private void OnKeyStatePersists(Key key)
        {
            if (this.orderedReceivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            foreach (IKeyStateReceiver receiver in this.orderedReceivers)
            {
                bool isExclusive = false;
                receiver.ReceivePersists(key, ref isExclusive);

                if (isExclusive)
                {
                    break;
                }
            }
        }
    }
}
