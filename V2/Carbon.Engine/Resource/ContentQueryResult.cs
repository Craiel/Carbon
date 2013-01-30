using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class ContentQueryResult<T> : ContentQueryResult
        where T : ICarbonContent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQueryResult(DbCommand command)
            : base(command)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<T> ToList<T>()
        {
            IList untyped = this.ToList(typeof(T));
            return untyped.Cast<T>().ToList();
        }
    }

    public class ContentQueryResult
    {
        private readonly DbCommand command;

        private IList entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQueryResult(DbCommand command)
        {
            this.command = command;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList ToList(Type type)
        {
            return this.entries;
        }
    }
}
