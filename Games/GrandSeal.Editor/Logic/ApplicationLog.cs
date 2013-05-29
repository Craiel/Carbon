using GrandSeal.Editor.Contracts;

using Core.Utils.Diagnostics;

namespace GrandSeal.Editor.Logic
{
    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("GrandSeal.Editor")
        {
        }
    }
}
