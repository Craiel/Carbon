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
        IReadOnlyCollection<string> Text { get; }

        void SetInputBindings(string name);
    }

    public class UserInterfaceConsole : UserInterfaceControl, IUserInterfaceConsole
    {
        private readonly ITypingController controller;
        private readonly IScriptingEngine scriptingEngine;

        private readonly List<string> buffer;

        private Lua consoleContext;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UserInterfaceConsole(IEngineFactory factory, ITypingController controller)
        {
            this.controller = controller;
            this.controller.OnReturnPressed += this.ControllerOnOnReturnPressed;

            this.scriptingEngine = factory.Get<IScriptingEngine>();
            this.scriptingEngine.Register(new ScriptingCoreProvider(factory.Get<IEngineLog>()));

            this.buffer = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
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

        public IReadOnlyCollection<string> Text
        {
            get
            {
                return this.buffer.AsReadOnly();
            }
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.consoleContext = this.scriptingEngine.GetContext();
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
        private void ControllerOnOnReturnPressed()
        {
            var newBuffer = this.controller.GetBuffer();
            foreach (string line in newBuffer)
            {
                this.buffer.Add(line);
                try
                {
                    object[] outData = this.consoleContext.DoString(line);
                    if (outData != null)
                    {
                        foreach (object o in outData)
                        {
                            this.buffer.Add(" -> Output: " + o);
                        }
                    }
                } 
                catch (Exception e)
                {
                    this.buffer.Add(" -> Error: " + e.Message);
                }
            }
        }
    }
}
