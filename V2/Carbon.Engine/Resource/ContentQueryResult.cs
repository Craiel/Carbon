using System.Collections.Generic;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    using System.Data.Common;
    using System.Data.SQLite;

    public class ContentQueryResult<T> where T : ICarbonContent
    {
        private readonly DbCommand command;

        private List<T> entries;

        public ContentQueryResult(DbCommand command)
        {
            this.command = command;
        }

        public IList<T> ToList()
        {
            return new List<T>(this.entries);
        }
    }
}
