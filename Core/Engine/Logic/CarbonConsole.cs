namespace Core.Engine.Logic
{
    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Rendering;

    using SharpDX;
    using SharpDX.DirectInput;

    public class CarbonConsole : EngineComponent, ICarbonConsole
    {
        private readonly ITypingController controller;
        
        private bool enableTimeStamp;

        private int maxLines;

        private Vector4 backgroundColor;
        
        private bool isVisible;

        private string text;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonConsole(IEngineFactory factory)
        {
            this.controller = factory.Get<ITypingController>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool EnableTimeStamp
        {
            get
            {
                return this.enableTimeStamp;
            }

            set
            {
                this.enableTimeStamp = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.controller.IsActive;
            }

            set
            {
                this.controller.IsActive = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value;
            }
        }

        public int MaxLines
        {
            get
            {
                return this.maxLines;
            }

            set
            {
                this.maxLines = value;
            }
        }

        public Vector4 BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                this.backgroundColor = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
        }

        public void Render(FrameInstructionSet activeSet)
        {
            throw new System.NotImplementedException();
        }

        public void ReceivePersists(Key key, ref bool isHandled)
        {
            throw new System.NotImplementedException();
        }

        public void ReceivePressed(Key key, ref bool isHandled)
        {
            throw new System.NotImplementedException();
        }

        public void ReceiveReleased(Key key, ref bool isHandled)
        {
            throw new System.NotImplementedException();
        }

        public void Write(string text)
        {
            throw new System.NotImplementedException();
        }

        public void WriteLine(string line)
        {
            throw new System.NotImplementedException();
        }
    }
}
