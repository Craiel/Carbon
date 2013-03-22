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
    }

    public class TypingController : EngineComponent, ITypingController
    {
        private readonly IKeyStateManager keyStateManager;

        private readonly List<string> buffer;
        private string lineBuffer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TypingController(IKeyStateManager keyStateManager)
        {
            this.keyStateManager = keyStateManager;
            this.keyStateManager.RegisterReceiver(this);

            this.buffer = new List<string>();
            this.lineBuffer = string.Empty;
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

                case Key.Return:
                    {
                        this.buffer.Add(this.lineBuffer);
                        this.lineBuffer = string.Empty;

                        if (this.OnReturnPressed != null)
                        {
                            this.OnReturnPressed();
                        }

                        break;
                    }

                case Key.Space:
                    {
                        this.lineBuffer += " ";
                        break;
                    }
                case Key.RightBracket:
                    {
                        this.lineBuffer += "(";
                        break;
                    }
                    case Key.LeftBracket:
                    {
                        this.lineBuffer += ")";
                        break;
                    }

                default:
                    {
                        var charString = key.ToString();
                        if (charString.Length != 1)
                        {
                            break;
                        }

                        char current = charString[0];
                        if (char.IsLetterOrDigit(current) || char.IsPunctuation(current) || char.IsSymbol(current))
                        {
                            this.lineBuffer += current;
                        }

                        break;
                    }
            }
        }

        public void ReceiveReleased(Key key, ref bool isHandled)
        {
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
    }
}
