using Carbed.Contracts;

using Core.Utils.Diagnostics;

namespace Carbed.Logic
{
    public class CarbedLog : LogBase, ICarbedLog
    {
        public CarbedLog()
            : base("Carbed")
        {
        }
    }
}
