namespace GrandSeal.Editor.Logic
{
    using CarbonCore.Utils.Diagnostics;

    using GrandSeal.Editor.Contracts;

    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("GrandSeal.Editor")
        {
        }
    }
}
