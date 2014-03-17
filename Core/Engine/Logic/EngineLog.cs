namespace Core.Engine.Logic
{
    using CarbonCore.Utils.Diagnostics;

    using Core.Engine.Contracts.Logic;

    public class EngineLog : LogBase, IEngineLog
    {
        public EngineLog()
            : base("Core.Engine")
        {
        }
    }
}
