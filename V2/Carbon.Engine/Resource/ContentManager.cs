using System;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Text;

    using Carbon.Engine.Contracts.Logic;

    using Core.Utils.Contracts;

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
        public ContentManager(IResourceManager resourceManager, IEngineLog log, ResourceLink rootResource)
        {
            this.resourceManager = resourceManager;
            this.log = log.AquireContextLog("ContentManager");

            this.rootResource = rootResource;
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
        private string BuildSelectStatement<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            string what = this.BuildSelect<T>();
            string where = this.BuildWhereClause<T>(criteria.Criterion);
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

        private string BuildSelect<T>() where T : ICarbonContent
        {
            string tableName = ContentReflection.GetTableName<T>();

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
        }

        private string BuildWhereClause<T>(IEnumerable<ContentCriterion> criteria)
        {
            
        }

        private string BuildOrder(IEnumerable<ContentOrder> orders)
        {
            
        }

        private void Connect()
        {
            if (this.connection != null)
            {
                return;
            }

            this.connection = this.factory.CreateConnection() as SQLiteConnection;
            
            // - Load the database from the resource manager
            // - Connect
            // - Create Schemas and check data
            // - If its not saved store it in the resource manager
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
