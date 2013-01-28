using System.Collections.Generic;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class ContentQueryResult<T> where T : ICarbonContent
    {
        private readonly IEnumerable<T> entries;

        ContentQueryResult(IEnumerable<T> entries)
        {
            this.entries = entries;
        }

        public IList<T> ToList()
        {
            return new List<T>(this.entries);
        }
    }
}
