using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic.Scripting;

using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public class InputManager : EngineComponent, IInputManager
    {
        private static readonly TimeSpan updateCycle = TimeSpan.FromMilliseconds(10);

        private readonly IList<IKeyStateReceiver> receivers;
        private readonly IDictionary<Key, bool> keyPressedState;

        private readonly IDictionary<string, InputBindings> bindings;

        private readonly DirectInput directInput;
        private readonly Keyboard keyboard;

        private TimeSpan lastUpdateTime;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputManager()
        {
            this.receivers = new List<IKeyStateReceiver>();
            this.keyPressedState = new Dictionary<Key, bool>();

            this.directInput = new DirectInput();
            this.keyboard = new Keyboard(directInput);
            this.keyboard.Acquire();

            this.bindings = new Dictionary<string, InputBindings>();
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
            }
        }

        public void UnregisterReceiver(IKeyStateReceiver receiver)
        {
            if (this.receivers.Contains(receiver))
            {
                this.receivers.Remove(receiver);
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
        public InputBindings RegisterBinding(string name)
        {
            if (this.bindings.ContainsKey(name))
            {
                throw new InvalidOperationException("Bindings with the same name are already registered");
            }

            var bindings = new InputBindings();
            this.bindings.Add(name, bindings);
            return bindings;
        }

        public InputBindings GetBindings(string name)
        {
            if (!this.bindings.ContainsKey(name))
            {
                throw new InvalidOperationException("No binding found for name " + name);
            }

            return this.bindings[name];
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnKeystateChange(Key key)
        {
            if(this.receivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            bool isPressed = this.keyPressedState[key];

            foreach (IKeyStateReceiver receiver in this.receivers)
            {
                if (isPressed)
                {
                    receiver.ReceivePressed(key);
                }
                else
                {
                    receiver.ReceiveReleased(key);
                }
            }
        }

        private void OnKeyStatePersists(Key key)
        {
            if (this.receivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            foreach (IKeyStateReceiver receiver in this.receivers)
            {
                receiver.ReceivePersists(key);
            }
        }
    }
}
