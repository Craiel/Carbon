using System;
using System.Collections.Generic;
using System.Linq;

using Carbon.Engine.Contracts.Logic;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public interface ITypingController : IBoundController
    {
        event Action OnReturnPressed;

        string Peek();

        string[] GetBuffer();
    }

    public class TypingController : BoundController, ITypingController
    {
        internal enum TypingControllerAction
        {
            Submit,
            Backspace,
        }

        private readonly IList<string> buffer;
        private string lineBuffer;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TypingController(IInputManager inputManager)
            : base(inputManager)
        {
            this.buffer = new List<string>();
            this.lineBuffer = string.Empty;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnReturnPressed;
        
        public string Peek()
        {
            return this.lineBuffer;
        }

        public string[] GetBuffer()
        {
            var values = this.buffer.ToArray();
            this.buffer.Clear();
            return values;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnBindingsTriggered(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (var binding in triggeredBindings)
            {
                TypingControllerAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    switch (action)
                    {
                        case TypingControllerAction.Submit:
                            {
                                this.buffer.Add(this.lineBuffer);
                                this.lineBuffer = string.Empty;

                                if (this.OnReturnPressed != null)
                                {
                                    this.OnReturnPressed();
                                }

                                break;
                            }

                        case TypingControllerAction.Backspace:
                            {
                                if (string.IsNullOrEmpty(this.lineBuffer))
                                {
                                    continue;
                                }

                                this.lineBuffer.Remove(this.lineBuffer.Length - 2);
                                break;
                            }
                    }

                    return;
                }

                this.lineBuffer += binding.Value;
            }
        }
    }
}
