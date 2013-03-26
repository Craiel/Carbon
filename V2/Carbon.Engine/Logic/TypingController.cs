using System;
using System.Collections.Generic;
using System.Linq;

using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public interface ITypingController : IBoundController
    {
        event Action OnReturnPressed;
        
        string[] GetBuffer();
    }

    public class TypingController : BoundController, ITypingController
    {
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
        
        public override void ReceivePressed(Key key)
        {
            if (!this.IsActive)
            {
                return;
            }
            
            switch (key)
            {
                case Key.Return:
                case Key.NumberPadEnter:
                    {
                        this.buffer.Add(this.lineBuffer);
                        this.lineBuffer = string.Empty;

                        if (this.OnReturnPressed != null)
                        {
                            this.OnReturnPressed();
                        }

                        break;
                    }
            }

            base.ReceivePressed(key);
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
                this.lineBuffer += binding.Value;
            }
        }
    }
}
