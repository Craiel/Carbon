using System;
using System.Collections.Generic;
using System.Diagnostics;

using Core.Utils.Contracts;

namespace Core.Utils.Diagnostics
{
    public sealed class ContextualLog : ILog
    {
        private readonly string context;
        private readonly ILog parent;

        public ContextualLog(string context, ILog parent)
        {
            this.context = context;
            this.parent = parent;
        }

        public void Warning(string message, params object[] args)
        {
            this.parent.Warning(string.Concat(this.context, "\t", message), args);
        }

        public void Error(string message, [System.Runtime.InteropServices.OptionalAttribute][System.Runtime.InteropServices.DefaultParameterValueAttribute(null)]Exception exception, params object[] args)
        {
            this.parent.Error(string.Concat(this.context, "\t", message), exception:exception, args:args);
        }

        public void Info(string message, params object[] args)
        {
            this.parent.Info(string.Concat(this.context, "\t", message), args);
        }

        public void Debug(string message, params object[] args)
        {
            this.parent.Debug(string.Concat(this.context, "\t", message), args);
        }
    }

    public abstract class LogBase : ILog
    {
        private readonly TraceSource traceSource;

        private readonly IDictionary<int, ILogEvent> registeredEvents;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected LogBase(string sourceName)
        {
            this.traceSource = new TraceSource(sourceName);

            this.registeredEvents = new Dictionary<int, ILogEvent>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Debug(string message, params object[] args)
        {
            string formattedMessage = message;
            if (args != null && args.Length > 0)
            {
                formattedMessage = string.Format(message, args);
            }

            System.Diagnostics.Trace.WriteLine(formattedMessage);
        }

        public void Warning(string message, params object[] args)
        {
            this.traceSource.TraceEvent(TraceEventType.Warning, -1, message, args);
        }

        public void Error(string message, Exception exception = null, params object[] args)
        {
            this.traceSource.TraceEvent(TraceEventType.Error, -1, message, args);
        }

        public void Info(string message, params object[] args)
        {
            this.traceSource.TraceInformation(message, args);
        }

        public void RegisterEvent(int id, ILogEvent data)
        {
            if (this.registeredEvents.ContainsKey(id))
            {
                throw new InvalidOperationException("Event already defined: " + id);
            }

            this.registeredEvents.Add(id, data);
        }

        public ILog AquireContextLog(string context)
        {
            return new ContextualLog(context, this);
        }
    }
}
