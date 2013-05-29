using Core.Engine.Contracts.Logic;

using Core.Utils.Diagnostics;

namespace Core.Engine.Logic
{
    public class EngineLog : LogBase, IEngineLog
    {
        public EngineLog()
            : base("Core.Engine")
        {
        }
    }
}
