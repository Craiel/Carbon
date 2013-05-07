using System;
using System.Collections.Generic;
using System.Linq;

using Carbon.Engine.Contracts.Logic;

namespace Carbon.Engine.Logic
{
    public interface ITypingController : IBoundController
    {
        event Action OnReturnPressed;
        event Func<string, string> OnCompletionRequested;

        string Peek();

        string[] GetBuffer();
    }

    public class TypingController : BoundController, ITypingController
    {
        internal enum TypingControllerAction
        {
            Submit,
            Backspace,
            Complete,
        }

        private readonly IList<string> buffer;
        private string lineBuffer;

        private string lastTrigger;
        private TimeSpan lastTriggerTime;
        private TimeSpan lastUpdateTime;
        private bool repeatThresholdReached;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TypingController(IInputManager inputManager)
            : base(inputManager)
        {
            this.buffer = new List<string>();
            this.lineBuffer = string.Empty;

            this.RepeatDelay = 250;
            this.RepeatThreshold = 600;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnReturnPressed;
        public event Func<string, string> OnCompletionRequested;

        public int RepeatDelay { get; set; }
        public int RepeatThreshold { get; set; }
        
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

        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            this.lastUpdateTime = gameTime.ActualElapsedTime;
            return true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnBindingsTriggeredPersist(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (var binding in triggeredBindings)
            {
                // Ignore persistent triggers unless we got a trigger before
                if (this.lastTrigger != binding.Value)
                {
                    continue;
                }

                this.HandleBinding(binding);
            }
        }

        protected override void OnBindingsTriggered(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (var binding in triggeredBindings)
            {
                this.HandleBinding(binding);
            }
        }

        protected override void OnBindingsTriggeredRelease(IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            this.lastTrigger = null;
            this.repeatThresholdReached = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckBindingRepetition(InputBindingEntry binding)
        {
            if (this.lastTrigger == binding.Value)
            {
                if (this.repeatThresholdReached)
                {
                    if ((this.lastUpdateTime - this.lastTriggerTime).TotalMilliseconds < this.RepeatDelay)
                    {
                        return false;
                    }
                }
                else
                {
                    if ((this.lastUpdateTime - this.lastTriggerTime).TotalMilliseconds < this.RepeatThreshold)
                    {
                        return false;
                    }

                    this.repeatThresholdReached = true;
                }
            }
            else
            {
                this.lastTrigger = binding.Value;
                this.lastTriggerTime = this.lastUpdateTime;
                this.repeatThresholdReached = false;
            }

            return true;
        }

        private void HandleBinding(InputBindingEntry binding)
        {
            if (!this.CheckBindingRepetition(binding))
            {
                return;
            }

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
                                return;
                            }

                            this.lineBuffer = this.lineBuffer.Remove(this.lineBuffer.Length - 1);
                            break;
                        }

                    case TypingControllerAction.Complete:
                        {
                            if (this.OnCompletionRequested != null)
                            {
                                this.lineBuffer = this.OnCompletionRequested(this.lineBuffer);
                            }

                            break;
                        }
                }

                return;
            }

            if (binding.Value.Length > 1 && binding.Value.StartsWith("#"))
            {
                int code;
                if (int.TryParse(binding.Value.Substring(1, binding.Value.Length - 1), out code))
                {
                    this.lineBuffer += (char)code;
                }
            }
            else
            {
                this.lineBuffer += binding.Value;
            }
        }
    }
}
