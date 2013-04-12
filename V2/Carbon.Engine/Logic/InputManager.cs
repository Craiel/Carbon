using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic.Scripting;

using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    using SlimDX;

    public class InputManager : EngineComponent, IInputManager
    {
        private static readonly TimeSpan updateCycle = TimeSpan.FromMilliseconds(10);

        private readonly IList<IInputReceiver> receivers;
        private readonly IDictionary<string, bool> pressedState;

        private readonly IDictionary<string, InputBindings> bindings;

        private readonly DirectInput directInput;
        private readonly Keyboard keyboard;
        private readonly Mouse mouse;

        private TimeSpan lastUpdateTime;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputManager()
        {
            this.receivers = new List<IInputReceiver>();
            this.pressedState = new Dictionary<string, bool>();

            this.directInput = new DirectInput();
            this.keyboard = new Keyboard(directInput);
            this.keyboard.Acquire();

            this.mouse = new Mouse(directInput);
            this.mouse.Acquire();

            this.bindings = new Dictionary<string, InputBindings>();
            this.SetupDefaultBindings();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.mouse.Dispose();
            this.keyboard.Dispose();
            this.directInput.Dispose();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector2 MinMousePosition { get; set; }
        public Vector2 MaxMousePosition { get; set; }

        public void RegisterReceiver(IInputReceiver receiver)
        {
            if (!this.receivers.Contains(receiver))
            {
                this.receivers.Add(receiver);
            }
        }

        public void UnregisterReceiver(IInputReceiver receiver)
        {
            if (this.receivers.Contains(receiver))
            {
                this.receivers.Remove(receiver);
            }
        }

        public override void Update(ITimer gameTimer)
        {
            if ((gameTimer.ActualElapsedTime - this.lastUpdateTime) < updateCycle)
            {
                return;
            }

            KeyboardState keyState = this.keyboard.GetCurrentState();
            MouseState mouseState = this.mouse.GetCurrentState();

            IList<string> previouslyPressed = this.pressedState.Keys.Where(entry => this.pressedState[entry]).ToList();
            IList<string> currentPressed = keyState.PressedKeys.Select(key => key.ToString()).ToList();

            bool[] buttons = mouseState.GetButtons();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i])
                {
                    currentPressed.Add(string.Format("Mouse{0}", i));
                }
            }

            foreach (string entry in currentPressed)
            {
                if (!this.pressedState.ContainsKey(entry))
                {
                    this.pressedState.Add(entry, false);
                }

                // Still pressed keys
                if (previouslyPressed.Contains(entry))
                {
                    previouslyPressed.Remove(entry);
                    this.OnStatePersists(entry);
                }

                // New pressed keys
                if (!this.pressedState[entry])
                {
                    this.pressedState[entry] = true;
                    this.OnStateChange(entry);
                }
            }

            // Check for released keys
            foreach (string entry in previouslyPressed)
            {
                this.pressedState[entry] = false;
                this.OnStateChange(entry);
            }

            // Process controller axis next
            IDictionary<string, float> axisChanges = new Dictionary<string, float>();
            if (mouseState.X < 0 || mouseState.X > 0)
            {
                axisChanges.Add("MouseX", mouseState.X);
            }

            if (mouseState.Y < 0 || mouseState.Y > 0)
            {
                axisChanges.Add("MouseY", mouseState.Y);
            }

            if (mouseState.Z < 0 || mouseState.Z > 0)
            {
                axisChanges.Add("MouseWheel", mouseState.Z);
            }

            foreach (string key in axisChanges.Keys)
            {
                foreach (IInputReceiver receiver in this.receivers)
                {
                    receiver.ReceiveAxisChange(key, axisChanges[key]);
                }
            }

            this.lastUpdateTime = gameTimer.ActualElapsedTime;
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

        [ScriptingMethod]
        public void UnregisterBinding(string name)
        {
            if (!this.bindings.ContainsKey(name))
            {
                return;
            }

            this.bindings.Remove(name);
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
        private void OnStateChange(string entry)
        {
            if(this.receivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            bool isPressed = this.pressedState[entry];

            foreach (IInputReceiver receiver in this.receivers)
            {
                if (isPressed)
                {
                    receiver.ReceivePressed(entry);
                }
                else
                {
                    receiver.ReceiveReleased(entry);
                }
            }
        }

        private void OnStatePersists(string entry)
        {
            if (this.receivers == null)
            {
                // No receivers registered yet, bail out
                return;
            }

            foreach (IInputReceiver receiver in this.receivers)
            {
                receiver.ReceivePersists(entry);
            }
        }

        private void SetupDefaultBindings()
        {
            var binding = this.RegisterBinding("debug");
            binding.BindEx("F3", "ToggleDebugOverlay", "PressAndRelease", "And");
            binding.BindEx("F9", "ToggleDepth", "PressAndRelease", "And");
            binding.BindEx("F10", "ToggleWireframe", "PressAndRelease", "And");

            binding = this.RegisterBinding("console");
            object[] capsModifiers = new[] { (object)"RightShift", "LeftShift" };
            binding.BindEx("A", "a", "Always", "And");
            binding.BindEx("A", "A", "Always", "Or", capsModifiers);
            binding.BindEx("B", "b", "Always", "And");
            binding.BindEx("B", "B", "Always", "Or", capsModifiers);
            binding.BindEx("C", "c", "Always", "And");
            binding.BindEx("C", "C", "Always", "Or", capsModifiers);
            binding.BindEx("D", "d", "Always", "And");
            binding.BindEx("D", "D", "Always", "Or", capsModifiers);
            binding.BindEx("E", "e", "Always", "And");
            binding.BindEx("E", "E", "Always", "Or", capsModifiers);
            binding.BindEx("F", "f", "Always", "And");
            binding.BindEx("F", "F", "Always", "Or", capsModifiers);
            binding.BindEx("G", "g", "Always", "And");
            binding.BindEx("G", "G", "Always", "Or", capsModifiers);
            binding.BindEx("H", "h", "Always", "And");
            binding.BindEx("H", "H", "Always", "Or", capsModifiers);
            binding.BindEx("I", "i", "Always", "And");
            binding.BindEx("I", "I", "Always", "Or", capsModifiers);
            binding.BindEx("J", "j", "Always", "And");
            binding.BindEx("J", "J", "Always", "Or", capsModifiers);
            binding.BindEx("K", "k", "Always", "And");
            binding.BindEx("K", "K", "Always", "Or", capsModifiers);
            binding.BindEx("L", "l", "Always", "And");
            binding.BindEx("L", "L", "Always", "Or", capsModifiers);
            binding.BindEx("M", "m", "Always", "And");
            binding.BindEx("M", "M", "Always", "Or", capsModifiers);
            binding.BindEx("N", "n", "Always", "And");
            binding.BindEx("N", "N", "Always", "Or", capsModifiers);
            binding.BindEx("O", "o", "Always", "And");
            binding.BindEx("O", "O", "Always", "Or", capsModifiers);
            binding.BindEx("P", "p", "Always", "And");
            binding.BindEx("P", "P", "Always", "Or", capsModifiers);
            binding.BindEx("Q", "q", "Always", "And");
            binding.BindEx("Q", "Q", "Always", "Or", capsModifiers);
            binding.BindEx("R", "r", "Always", "And");
            binding.BindEx("R", "R", "Always", "Or", capsModifiers);
            binding.BindEx("S", "s", "Always", "And");
            binding.BindEx("S", "S", "Always", "Or", capsModifiers);
            binding.BindEx("T", "t", "Always", "And");
            binding.BindEx("T", "T", "Always", "Or", capsModifiers);
            binding.BindEx("U", "u", "Always", "And");
            binding.BindEx("U", "U", "Always", "Or", capsModifiers);
            binding.BindEx("V", "v", "Always", "And");
            binding.BindEx("V", "V", "Always", "Or", capsModifiers);
            binding.BindEx("W", "w", "Always", "And");
            binding.BindEx("W", "W", "Always", "Or", capsModifiers);
            binding.BindEx("X", "x", "Always", "And");
            binding.BindEx("X", "X", "Always", "Or", capsModifiers);
            binding.BindEx("Y", "y", "Always", "And");
            binding.BindEx("Y", "Y", "Always", "Or", capsModifiers);
            binding.BindEx("Z", "z", "Always", "And");
            binding.BindEx("Z", "Z", "Always", "Or", capsModifiers);

            binding.BindEx("D0", "#48", "Always", "And");
            binding.BindEx("D0", ")", "Always", "Or", capsModifiers);
            binding.BindEx("D1", "#49", "Always", "And");
            binding.BindEx("D1", "!", "Always", "Or", capsModifiers);
            binding.BindEx("D2", "#50", "Always", "And");
            binding.BindEx("D2", "@", "Always", "Or", capsModifiers);
            binding.BindEx("D3", "#51", "Always", "And");
            binding.BindEx("D3", "#", "Always", "Or", capsModifiers);
            binding.BindEx("D4", "#52", "Always", "And");
            binding.BindEx("D4", "@", "Always", "Or", capsModifiers);
            binding.BindEx("D5", "#53", "Always", "And");
            binding.BindEx("D5", "%", "Always", "Or", capsModifiers);
            binding.BindEx("D6", "#54", "Always", "And");
            binding.BindEx("D6", "^", "Always", "Or", capsModifiers);
            binding.BindEx("D7", "#55", "Always", "And");
            binding.BindEx("D7", "&", "Always", "Or", capsModifiers);
            binding.BindEx("D8", "#56", "Always", "And");
            binding.BindEx("D8", "*", "Always", "Or", capsModifiers);
            binding.BindEx("D9", "#57", "Always", "And");
            binding.BindEx("D9", "(", "Always", "Or", capsModifiers);

            binding.BindEx("Return", "Submit", "Always", "And");
            binding.BindEx("NumberPadEnter", "Submit", "Always", "And");
            binding.BindEx("Backspace", "Backspace", "Always", "And");
            binding.BindEx("Tab", "Complete", "Always", "And");
            
            binding.BindEx("LeftBracket", "[", "Always", "And");
            binding.BindEx("LeftBracket", "{", "Always", "Or", capsModifiers);
            binding.BindEx("RightBracket", "]", "Always", "And");
            binding.BindEx("RightBracket", "}", "Always", "Or", capsModifiers);
            binding.BindEx("Comma", ",", "Always", "And");
            binding.BindEx("Comma", "<", "Always", "Or", capsModifiers);
            binding.BindEx("Semicolon", ";", "Always", "And");
            binding.BindEx("Semicolon", ":", "Always", "Or", capsModifiers);
            binding.BindEx("Apostrophe", "'", "Always", "And");
            binding.BindEx("Apostrophe", "\"", "Always", "Or", capsModifiers);
            binding.BindEx("Backslash", "\\", "Always", "And");
            binding.BindEx("Backslash", "|", "Always", "Or", capsModifiers);
            binding.BindEx("Space", " ", "Always", "And");
            binding.BindEx("Slash", "/", "Always", "And");
            binding.BindEx("Slash", "?", "Always", "Or", capsModifiers);
            binding.BindEx("Equals", "=", "Always", "And");
            binding.BindEx("Equals", "+", "Always", "Or", capsModifiers);
            binding.BindEx("Minus", "-", "Always", "And");
            binding.BindEx("Minus", "_", "Always", "Or", capsModifiers);
            binding.BindEx("Period", ".", "Always", "And");
            binding.BindEx("Period", ">", "Always", "Or", capsModifiers);

            binding.BindEx("NumberPad0", "#48", "Always", "And");
            binding.BindEx("NumberPad1", "#49", "Always", "And");
            binding.BindEx("NumberPad2", "#50", "Always", "And");
            binding.BindEx("NumberPad3", "#51", "Always", "And");
            binding.BindEx("NumberPad4", "#52", "Always", "And");
            binding.BindEx("NumberPad5", "#53", "Always", "And");
            binding.BindEx("NumberPad6", "#54", "Always", "And");
            binding.BindEx("NumberPad7", "#55", "Always", "And");
            binding.BindEx("NumberPad8", "#56", "Always", "And");
            binding.BindEx("NumberPad9", "#57", "Always", "And");

            binding.BindEx("NumberPadMinus", "-", "Always", "And");
            binding.BindEx("NumberPadSlash", "/", "Always", "And");
            binding.BindEx("NumberPadStar", "*", "Always", "And");
            binding.BindEx("NumberPadPlus", "+", "Always", "And");
            binding.BindEx("NumberPadPeriod", ".", "Always", "And");
        }
    }
}
