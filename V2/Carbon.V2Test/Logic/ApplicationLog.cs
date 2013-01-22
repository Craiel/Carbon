using Carbon.V2Test.Contracts;

using Core.Utils.Diagnostics;

namespace Carbon.V2Test.Logic
{
    public class ApplicationLog : LogBase, IApplicationLog
    {
        public ApplicationLog()
            : base("Carbon.V2Test")
        {
        }
    }
}
