namespace GrandSeal.Logic
{
    using CarbonCore.Utils.Diagnostics;

    using Contracts;
    
    public class GrandSealLog : LogBase, IGrandSealLog
    {
        public GrandSealLog()
            : base("GrandSeal")
        {
        }
    }
}
