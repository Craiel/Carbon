﻿using System;
using System.Collections.Generic;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.UserInterface;
using Carbon.Engine.Logic;

using Core.Utils.Contracts;

namespace Carbon.Engine.UserInterface
{
    public class UserInterfaceConsole : UserInterfaceControl, IUserInterfaceConsole
    {
        private readonly ITypingController controller;

        private readonly List<string> buffer;

        private readonly IFormatter formatter;

        // Todo: Move these into engine settings for defaults and override-able via lua scripting
        private int maxLines = 100;
        private int maxCharactersPerLine = 70;
        private string lineFormat = "[{DATETIME:t}] {LINE}";
        private string systemLineFormat = " {LINE}";
        private string textFormat = "> {LINE}";

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UserInterfaceConsole(IEngineFactory factory, ITypingController controller)
        {
            this.controller = controller;
            this.controller.OnReturnPressed += this.ControllerOnReturnPressed;
            this.controller.OnCompletionRequested += this.ControllerOnCompletionRequested;
            this.controller.SetInputBindings("console");

            this.formatter = factory.Get<IFormatter>();

            this.buffer = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<string> OnLineEntered;
        public event Func<string, string> OnRequestCompletion;

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

        public int MaxCharactersPerLine
        {
            get
            {
                return this.maxCharactersPerLine;
            }

            set
            {
                this.maxCharactersPerLine = value;
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
                this.formatter.Set("LINE", this.controller.Peek());
                return this.formatter.Format(textFormat);
            }
        }
        
        public override void Update(ITimer gameTime)
        {
            base.Update(gameTime);

            if (!this.IsActive)
            {
                return;
            }

            this.controller.Update(gameTime);
        }

        public void SetInputBindings(string name)
        {
            this.controller.SetInputBindings(name);
        }

        public void AddSystemLine(string line)
        {
            this.AddHistory(line, this.systemLineFormat);
        }

        public void AddLine(string line)
        {
            this.AddHistory(line, this.lineFormat);
        }

        // -------------------------------------------------------------------
        // private
        // -------------------------------------------------------------------
        private void ControllerOnReturnPressed()
        {
            var newBuffer = this.controller.GetBuffer();
            foreach (string line in newBuffer)
            {
                this.AddHistory(line, this.lineFormat);
                if (this.OnLineEntered != null)
                {
                    this.OnLineEntered(line);
                }
            }
        }

        private string ControllerOnCompletionRequested(string arg)
        {
            if (this.OnRequestCompletion != null)
            {
                return this.OnRequestCompletion(arg);
            }

            return arg;
        }

        private void AddHistory(string line, string template)
        {
            IList<string> linesToAdd = new List<string>();
            while (line.Length > this.maxCharactersPerLine)
            {
                linesToAdd.Add(line.Substring(0, this.maxCharactersPerLine));
                line = line.Substring(this.maxCharactersPerLine, line.Length - this.maxCharactersPerLine);
            }

            linesToAdd.Add(line);

            while (this.buffer.Count + linesToAdd.Count > this.maxLines)
            {
                this.buffer.RemoveAt(0);
            }

            foreach (string newLine in linesToAdd)
            {
                this.formatter.Set("LINE", newLine);
                string formattedLine = this.formatter.Format(template);
                this.buffer.Add(formattedLine);
            }
        }
    }
}