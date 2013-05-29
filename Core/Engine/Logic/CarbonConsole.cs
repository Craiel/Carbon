using Core.Engine.Contracts;
using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Rendering;
using Core.Engine.Rendering;

using SlimDX;
using SlimDX.DirectInput;

namespace Core.Engine.Logic
{
    public interface ICarbonConsole : IEngineComponent, IRenderable
    {
        bool EnableTimeStamp { get; set; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }

        int MaxLines { get; set; }

        Vector4 BackgroundColor { get; set; }

        string Text { get; }

        void Write(string text);
        void WriteLine(string line);
    }

    public class CarbonConsole : EngineComponent, ICarbonConsole
    {
        private readonly ITypingController controller;

        private string currentLine;

        private bool enableTimeStamp;

        private int maxLines;

        private Vector4 backgroundColor;

        private string text;
        
        private bool isVisible;

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
