namespace Carbon.Engine.UserInterface
{
    using System;
    using System.Collections.Generic;

    using Carbon.Engine.Contracts;
    using Carbon.Engine.Contracts.Logic;
    using Carbon.Engine.Logic;
    using Carbon.Engine.Logic.Scripting;

    using LuaInterface;

    public interface IUserInterfaceConsole : IUserInterfaceControl
    {
        event Action<string> OnLineEntered;

        int MaxLines { get; set; }

        string LineFormat { get; set; }

        IReadOnlyCollection<string> History { get; }
        string Text { get; }

        void SetInputBindings(string name);
    }

    public class UserInterfaceConsole : UserInterfaceControl, IUserInterfaceConsole
    {
        private readonly ITypingController controller;

        private readonly List<string> buffer;

        // Todo: Move these into engine settings for defaults and overridable via lua scripting
        // Todo: Use the utils formatter for this so we can easily use datetime and other things in the console too
        private int maxLines = 10;
        private string lineFormat = "> {0}";

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UserInterfaceConsole(IEngineFactory factory, ITypingController controller)
        {
            this.controller = controller;
            this.controller.OnReturnPressed += this.ControllerOnReturnPressed;
            this.controller.SetInputBindings("console");
            
            this.buffer = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<string> OnLineEntered;

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

        public string LineFormat
        {
            get
            {
                return this.lineFormat;
            }
            set
            {
                this.lineFormat = value;
            }
        }

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }

            set
            {
                base.IsActive = value;
                this.controller.IsActive = value;
            }
        }

        public IReadOnlyCollection<string> History
        {
            get
            {
                return this.buffer.AsReadOnly();
            }
        }

        public string Text
        {
            get
            {
                return string.Format(this.LineFormat, string.Join(Environment.NewLine, this.controller.Peek()));
            }
        }
        
        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            if (!this.IsActive)
            {
                return;
            }
        }

        public void SetInputBindings(string name)
        {
            this.controller.SetInputBindings(name);
        }

        // -------------------------------------------------------------------
        // private
        // -------------------------------------------------------------------
        private void ControllerOnReturnPressed()
        {
            var newBuffer = this.controller.GetBuffer();
            foreach (string line in newBuffer)
            {
                this.buffer.Add(line);
                if (this.OnLineEntered != null)
                {
                    this.OnLineEntered(line);
                }
            }
        }
    }
}
