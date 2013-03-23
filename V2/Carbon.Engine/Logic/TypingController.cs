using System.Linq;

using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    using System;
    using System.Collections.Generic;

    public interface ITypingController : IEngineComponent, IKeyStateReceiver
    {
        event Action OnReturnPressed;

        bool IsActive { get; set; }

        string[] GetBuffer();

        void SetInputBindings(string name);
    }

    public class TypingController : EngineComponent, ITypingController
    {
        private readonly IKeyStateManager keyStateManager;

        private readonly IList<string> buffer;
        private readonly IList<Key> modifiers;
        private string lineBuffer;

        private KeyBindings bindings;

        /*private static readonly IDictionary<Key, char[]> TypeCharMap = new Dictionary<Key, char[]>
                                                                {
                                                                    { Key.A, new[] { 'a', 'A' } },
                                                                    { Key.B, new[] { 'b', 'B' } },
                                                                    { Key.C, new[] { 'c', 'C' } },
                                                                    { Key.D, new[] { 'd', 'D' } },
                                                                    { Key.E, new[] { 'e', 'E' } },
                                                                    { Key.F, new[] { 'f', 'F' } },
                                                                    { Key.G, new[] { 'g', 'G' } },
                                                                    { Key.H, new[] { 'h', 'H' } },
                                                                    { Key.I, new[] { 'i', 'I' } },
                                                                    { Key.J, new[] { 'j', 'J' } },
                                                                    { Key.K, new[] { 'k', 'K' } },
                                                                    { Key.L, new[] { 'l', 'L' } },
                                                                    { Key.M, new[] { 'm', 'M' } },
                                                                    { Key.N, new[] { 'n', 'N' } },
                                                                    { Key.O, new[] { 'o', 'O' } },
                                                                    { Key.P, new[] { 'p', 'P' } },
                                                                    { Key.Q, new[] { 'q', 'Q' } },
                                                                    { Key.R, new[] { 'r', 'R' } },
                                                                    { Key.S, new[] { 's', 'S' } },
                                                                    { Key.T, new[] { 't', 'T' } },
                                                                    { Key.U, new[] { 'u', 'U' } },
                                                                    { Key.V, new[] { 'v', 'V' } },
                                                                    { Key.W, new[] { 'w', 'W' } },
                                                                    { Key.X, 'x' },
                                                                    { Key.Y, 'y' },
                                                                    { Key.Z, 'z' },
                                                                    { Key.D0, '0' },
                                                                    { Key.D1, '1' },
                                                                    { Key.D2, '2' },
                                                                    { Key.D3, '3' },
                                                                    { Key.D4, '4' },
                                                                    { Key.D5, '5' },
                                                                    { Key.D6, '6' },
                                                                    { Key.D7, '7' },
                                                                    { Key.D8, '8' },
                                                                    { Key.D9, '9' },
                                                                    { Key.LeftBracket, '[' },
                                                                    { Key.RightBracket, ']' },
                                                                    { Key.Comma, ',' },
                                                                    { Key.Colon, ';' },
                                                                    { Key.Apostrophe, '\'' },
                                                                    { Key.Backslash, '\\' },
                                                                    { Key.Tab, '\t' },
                                                                    { Key.Space , ' ' },
                                                                    { Key.Slash, '/' },
                                                                    { Key.Equals, '=' },
                                                                    { Key.Minus, '-' },
                                                                    { Key.Period, '.' },
                                                                    { Key.NumberPad0, '0' },
                                                                    { Key.NumberPad1, '1' },
                                                                    { Key.NumberPad2, '2' },
                                                                    { Key.NumberPad3, '3' },
                                                                    { Key.NumberPad4, '4' },
                                                                    { Key.NumberPad5, '5' },
                                                                    { Key.NumberPad6, '6' },
                                                                    { Key.NumberPad7, '7' },
                                                                    { Key.NumberPad8, '8' },
                                                                    { Key.NumberPad9, '9' },
                                                                    { Key.NumberPadMinus, '-' },
                                                                    { Key.NumberPadSlash, '/' },
                                                                    { Key.NumberPadStar, '*' },
                                                                    { Key.NumberPadPlus, '+' },
                                                                    { Key.NumberPadPeriod, '.' },
                                                                };*/

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TypingController(IKeyStateManager keyStateManager)
        {
            this.keyStateManager = keyStateManager;
            this.keyStateManager.RegisterReceiver(this);

            this.buffer = new List<string>();
            this.lineBuffer = string.Empty;

            this.modifiers = new List<Key>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnReturnPressed;

        public bool IsActive { get; set; }

        public int Order
        {
            get
            {
                return 100;
            }
        }
        
        public void ReceivePersists(Key key, ref bool isHandled)
        {
        }
        
        public void ReceivePressed(Key key, ref bool isHandled)
        {
            if (!this.IsActive)
            {
                return;
            }

            switch (key)
            {
                // Add anything extra here
                case Key.RightShift:
                case Key.LeftShift:
                case Key.RightControl:
                case Key.LeftControl:
                    {
                        this.modifiers.Add(key);
                        isHandled = true;
                        break;
                    }

                case Key.Return:
                case Key.NumberPadEnter:
                    {
                        this.buffer.Add(this.lineBuffer);
                        this.lineBuffer = string.Empty;
                        isHandled = true;

                        if (this.OnReturnPressed != null)
                        {
                            this.OnReturnPressed();
                        }

                        break;
                    }

                default:
                    {
                        var keyBindings = this.bindings.GetBindings(key);
                        if (keyBindings != null && keyBindings.Length > 0)
                        {
                            foreach (KeyBindingEntry binding in keyBindings)
                            {
                                if (this.MatchModifiers(binding))
                                {
                                    this.lineBuffer += binding.Value;
                                }
                            }
                        }

                        break;
                    }
            }
        }

        public void ReceiveReleased(Key key, ref bool isHandled)
        {
            if (!this.IsActive)
            {
                return;
            }

            switch (key)
            {
                    // Add anything extra here
                case Key.RightShift:
                case Key.LeftShift:
                case Key.RightControl:
                case Key.LeftControl:
                    {
                        this.modifiers.Remove(key);
                        isHandled = true;
                        break;
                    }
            }
        }

        public override void Update(ITimer gameTime)
        {
        }

        public override void Dispose()
        {
            this.keyStateManager.UnregisterReceiver(this);

            base.Dispose();
        }
       
        public string[] GetBuffer()
        {
            var values = this.buffer.ToArray();
            this.buffer.Clear();
            return values;
        }

        public void SetInputBindings(string name)
        {
            this.bindings = this.keyStateManager.GetBindings(name);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool MatchModifiers(KeyBindingEntry entry)
        {
            if (entry.Modifiers == null)
            {
                if (this.modifiers.Count <= 0)
                {
                    return true;
                }

                return false;
            }

            if (entry.ModifierMode == KeyBindingModifierMode.And)
            {
                return entry.Modifiers.SequenceEqual(this.modifiers);
            }

            return this.modifiers.Any(x => entry.Modifiers.Contains(x));
        }
    }
}
