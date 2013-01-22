using Carbon.Engine.Contracts.Logic;

using Core.Utils.Diagnostics;

namespace Carbon.Engine.Logic
{
    public class EngineLog : LogBase, IEngineLog
    {
        public EngineLog()
            : base("Carbon.Engine")
        {
        }
    }
}
