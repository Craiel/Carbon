namespace Core.Utils.Diagnostics
{
    using Core.Utils.Contracts;

    public class LogEvent : ILogEvent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Message { get; set; }
    }
}
