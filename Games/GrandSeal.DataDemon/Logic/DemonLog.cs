using Core.Utils.Diagnostics;

namespace GrandSeal.DataDemon.Logic
{
    using GrandSeal.DataDemon.Contracts;

    public class DemonLog : LogBase, IDemonLog
    {
        public DemonLog()
            : base("GrandSeal.DataDemon")
        {
        }
    }
}
