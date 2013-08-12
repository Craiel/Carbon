namespace GrandSeal.Logic
{
    using Contracts;

    using Core.Utils.Diagnostics;

    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("GrandSeal")
        {
        }
    }
}
