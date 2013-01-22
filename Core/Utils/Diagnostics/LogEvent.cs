using Core.Utils.Contracts;

namespace Core.Utils.Diagnostics
{
    public class LogEvent : ILogEvent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Message { get; set; }
    }
}
