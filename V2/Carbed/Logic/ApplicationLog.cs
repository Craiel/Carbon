using Carbed.Contracts;

using Core.Utils.Diagnostics;

namespace Carbed.Logic
{
    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("Carbed")
        {
        }
    }
}
