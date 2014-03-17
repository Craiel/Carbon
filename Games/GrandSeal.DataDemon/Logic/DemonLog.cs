namespace GrandSeal.DataDemon.Logic
{
    using CarbonCore.Utils.Diagnostics;

    using GrandSeal.DataDemon.Contracts;

    public class DemonLog : LogBase, IDemonLog
    {
        public DemonLog()
            : base("GrandSeal.DataDemon")
        {
        }
    }
}
