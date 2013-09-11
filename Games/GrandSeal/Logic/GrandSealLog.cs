namespace GrandSeal.Logic
{
    using Contracts;

    using Core.Utils.Diagnostics;

    public class GrandSealLog : LogBase, IGrandSealLog
    {
        public GrandSealLog()
            : base("GrandSeal")
        {
        }
    }
}
