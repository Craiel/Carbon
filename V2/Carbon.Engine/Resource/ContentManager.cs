using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;

using Core.Utils.Contracts;

namespace Carbon.Engine.Resource
{
    public class ContentManager : IContentManager
    {
        private readonly IResourceManager resourceManager;
        private readonly ILog log;

        private readonly ResourceLink rootResource;

        private readonly SQLiteFactory factory;

        private SQLiteConnection connection;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentManager(IResourceManager resourceManager, IEngineLog log)
        {
            this.resourceManager = resourceManager;
            this.log = log.AquireContextLog("ContentManager");

            this.factory = new SQLiteFactory();
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ContentQueryResult<T> Load<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            return this.Load(criteria);
        }

        public ContentQueryResult Load(ContentQuery criteria)
        {
            this.Connect();

            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = this.BuildSelectStatement(criteria);

            this.log.Debug("ConentManager.Load<{0}>: {1}", typeof(T), command.CommandText);
            return new ContentQueryResult<T>(command);
        }

        public void Save(ICarbonContent content)
        {
            this.Connect();

            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string BuildSelectStatement(ContentQuery criteria)
        {
            string what = this.BuildSelect();
            string where = this.BuildWhereClause(criteria.Criterion);
            string order = this.BuildOrder(criteria.Order);

            return this.AssembleStatement(what, where, order);
        }

        private string AssembleStatement(string what, string where, string order)
        {
            StringBuilder builder = new StringBuilder(what);
            if (!string.IsNullOrEmpty(where))
            {
                builder.AppendFormat(" WHERE {0}", where);
            }

            if(!string.IsNullOrEmpty(order))
            {
                builder.AppendFormat(" ORDER BY {0}", order);
            }

            return builder.ToString();
        }

        private string BuildSelect()
        {
            string tableName = ContentReflection.GetTableName();

            ContentReflection.GetPropertyInfos<T>();
            /*if (QueryCache.ContainsKey(targetType))
            {
                return QueryCache[targetType];
            }

            string tableName = LookupDataReflection.GetTableName(targetType);
            IEnumerable<LookupColumnInfo> columnInfos = LookupDataReflection.GetColumnInfo(targetType);

            StringBuilder columnQueryBuilder = new StringBuilder();
            foreach (LookupColumnInfo lookupColumnInfo in columnInfos)
            {
                if (columnQueryBuilder.Length > 0)
                {
                    columnQueryBuilder.Append(",");
                }

                columnQueryBuilder.Append(lookupColumnInfo.ColumnName);
            }

            string query = string.Format("select {0} from {1}", columnQueryBuilder, tableName);
            QueryCache.Add(targetType, query);
            return query;*/

            return null;
        }

        private string BuildWhereClause<T>(IEnumerable<ContentCriterion> criteria)
        {
            return null;
        }

        private string BuildOrder(IEnumerable<ContentOrder> orders)
        {
            return null;
        }

        private void Connect()
        {
            if (this.connection != null)
            {
                return;
            }

            // Todo:
            // - Load the database from the resource manager
            // - Pull the data into the memory database
            // - Release the resource
            this.connection = (SQLiteConnection)this.factory.CreateConnection();
            this.connection.ConnectionString = "Data Source=:memory:";
            this.connection.Open();

            // Todo:
            // - Create Schema data
        }

        private void Disconnect()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }
    }
}
