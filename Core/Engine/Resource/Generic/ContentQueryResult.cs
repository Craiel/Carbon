namespace Core.Engine.Resource.Generic
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    using CarbonCore.Utils.Contracts;

    using Core.Engine.Contracts.Resource;

    public class ContentQueryResult<T> : ContentQueryResult
        where T : ICarbonContent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQueryResult(IContentManager contentManager, ILog log, DbCommand command)
            : base(contentManager, log, command)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<T> ToList()
        {
            IList untyped = this.ToList(typeof(T));
            return untyped.Cast<T>().ToList();
        }

        public new T UniqueResult()
        {
            return (T)this.UniqueResult(typeof(T));
        }
    }
}
