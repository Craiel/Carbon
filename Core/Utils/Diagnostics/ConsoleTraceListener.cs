﻿using System.Diagnostics;
using System.Globalization;

using Core.Utils.Contracts;

namespace Core.Utils.Diagnostics
{
    using Core.Utils.Formatting;

    // Taken in large parts from Essential Diagnostics Project
    public class ConsoleTraceListener : System.Diagnostics.ConsoleTraceListener
    {
        private const string DefaultTemplate = "{DateTime:u}\t{Source}({ThreadId})\t{EventType}\t{Id}\t{Message}";

        private readonly IFormatter formatter;

        private string template = DefaultTemplate;

        private bool attributesProcessed;

        public ConsoleTraceListener()
        {
            this.formatter = new Formatter();
        }
        
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (!this.attributesProcessed)
            {
                this.ProcessAttributes();
            }
            
            this.formatter.Set("Source", source);
            this.formatter.Set("EventType", eventType.ToString());
            this.formatter.Set("Id", id.ToString(CultureInfo.InvariantCulture));
            this.formatter.Set("Message", args.Length > 0 ? string.Format(format, args) : format);

            System.Console.WriteLine(this.formatter.Format(this.template));
        }

        public override void Write(string message)
        {
            this.formatter.Set("Message", message);
            System.Console.Write(this.formatter.Format(this.template));
        }

        public override void WriteLine(string message)
        {
            this.formatter.Set("Message", message);
            System.Console.WriteLine(this.formatter.Format(this.template));
        }

        protected override string[] GetSupportedAttributes()
        {
            return new[] { "template", "Template" };
        }

        private void ProcessAttributes()
        {
            lock (this.Attributes)
            {
                this.attributesProcessed = true;

                if (this.Attributes.ContainsKey("template"))
                {
                    this.template = this.Attributes["template"];
                    this.template = this.template.Replace("\\t", "\t"); // Gets escaped so we have to reverse this, can't see a better way atm
                }
            }
        }
    }
}
