using GrandSeal.Contracts;
using Core.Utils.Diagnostics;

namespace GrandSeal.Logic
{
    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("GrandSeal")
        {
        }
    }
}
