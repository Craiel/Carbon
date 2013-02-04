using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;

using Core.Utils.Contracts;

namespace Carbon.Engine.Resource
{
    public class ContentManager : IContentManager
    {
        private const string SqlNotNull = " NOT NULL";

        private readonly IResourceManager resourceManager;
        private readonly ILog log;

        private readonly ResourceLink root;

        private readonly SQLiteFactory factory;
        private readonly IList<string> checkedTableList;

        private SQLiteConnection connection;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentManager(IResourceManager resourceManager, IEngineLog log, ResourceLink root)
        {
            this.resourceManager = resourceManager;
            this.log = log.AquireContextLog("ContentManager");
            this.root = root;

            this.factory = new SQLiteFactory();
            this.checkedTableList = new List<string>();
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            return this.Load(criteria) as ContentQueryResult<T>;
        }

        public ContentQueryResult Load(ContentQuery criteria)
        {
            this.Connect();
            this.CheckTable(criteria.Type);

            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = this.BuildSelectStatement(criteria);

            this.log.Debug("ConentManager.Load<{0}>: {1}", criteria.Type, command.CommandText);
            return new ContentQueryResult(command);
        }

        public void Save(ICarbonContent content)
        {
            this.Connect();
            this.CheckTable(content.GetType());

            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = this.BuildInsertStatement(content);
            this.log.Debug("ContentManager.Save<{0}>: {1}", content.GetType(), command.CommandText);
            int affected = command.ExecuteNonQuery();
            if (affected != 1)
            {
                throw new InvalidOperationException("Expected 1 row affected but got " + affected);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string BuildSelectStatement(ContentQuery criteria)
        {
            string what = this.BuildSelect(criteria.Type);
            string where = this.BuildWhereClause(criteria.Criterion);
            string order = this.BuildOrder(criteria.Order);

            return this.AssembleStatement(what, where, order);
        }

        private string BuildInsertStatement(ICarbonContent content)
        {
            string insert = this.BuildInsert(content.GetType());
            string what = this.BuildInsertValues(content);
            return string.Format("{0} VALUES ({1})", insert, what);
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

        private string BuildSelect(Type targetType)
        {
            string tableName = ContentReflection.GetTableName(targetType);

            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            StringBuilder builder = new StringBuilder();
            foreach (ContentReflectionProperty property in properties)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(property.Name);
            }

            return string.Format("SELECT {0} FROM {1}", builder, tableName);
        }

        private string BuildInsert(Type targetType)
        {
            string tableName = ContentReflection.GetTableName(targetType);

            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            StringBuilder builder = new StringBuilder();
            foreach (ContentReflectionProperty property in properties)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(property.Name);
            }

            return string.Format("INSERT INTO {0} ({1})", tableName, builder);
        }

        private string BuildWhereClause(IEnumerable<ContentCriterion> criteria)
        {
            IList<string> segments = new List<string>();
            foreach (ContentCriterion criterion in criteria)
            {
                if (criterion.Value == null)
                {
                    segments.Add(
                        criterion.Negate
                            ? string.Format("{0} IS NOT NULL", criterion.PropertyInfo.Name)
                            : string.Format("{0} IS NULL", criterion.PropertyInfo.Name));
                }
                else
                {
                    string segmentString = string.Format("{0} = '{1}'", criterion.PropertyInfo.Name, this.TranslateCriterionValue(criterion.Value));
                    if (criterion.Negate)
                    {
                        segmentString = string.Concat("NOT ", segmentString);
                    }

                    segments.Add(segmentString);
                }
            }

            return string.Join(" AND ", segments);
        }

        private string BuildInsertValues(ICarbonContent entry)
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(entry.GetType());
            IList<string> segments = new List<string>();
            foreach (ContentReflectionProperty property in properties)
            {
                object value = property.Info.GetValue(entry);
                segments.Add(this.TranslateInsertValue(value));
            }

            return string.Join(", ", segments);
        }

        private string TranslateInsertValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }

            Type type = value.GetType();
            if (type == typeof(ResourceLink))
            {
                return ((ResourceLink)value).Id.ToString();
            }

            return string.Format("'{0}'", value);
        }

        private string TranslateCriterionValue(object value)
        {
            return value.ToString();
        }

        private string BuildOrder(IEnumerable<ContentOrder> orders)
        {
            IList<string> segments = new List<string>();
            foreach (ContentOrder order in orders)
            {
                segments.Add(string.Format("ORDER BY {0} {1}", order.PropertyInfo.Name, order.Ascending ? "ASC" : "DESC"));
            }

            return string.Join(" ", segments);
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
            //this.connection.ConnectionString = "Data Source=:memory:";
            this.connection.ConnectionString = string.Format("Data Source={0}", this.root.Source);
            this.connection.Open();
        }

        private void Disconnect()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }

            this.checkedTableList.Clear();
        }

        private void CheckTable(Type type)
        {
            string tableName = ContentReflection.GetTableName(type);
            if (this.checkedTableList.Contains(tableName))
            {
                return;
            }

            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(type);
            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = string.Format("PRAGMA table_info({0})", tableName);
            IList<object[]> pragmaInfo = new List<object[]>();
            using (SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    if (reader.GetValues(data) != reader.FieldCount)
                    {
                        throw new InvalidOperationException("GetValues returned unexpected field count");
                    }

                    pragmaInfo.Add(data);
                }
            }

            bool needRecreate = properties.Count != pragmaInfo.Count;
            IList<string> columnSegments = new List<string>();
            IList<string> primaryKeySegments = new List<string>();
            for (int i = 0; i < properties.Count; i++)
            {
                string tableType = this.GetTableType(properties[i].Info.PropertyType);
                string tableColumn = properties[i].Name;

                if (properties[i].PrimaryKey)
                {
                    // If the column is part of the primary key we have to mark it as not null
                    if (!tableType.Contains(SqlNotNull))
                    {
                        tableType = string.Concat(tableType, SqlNotNull);
                    }

                    primaryKeySegments.Add(tableColumn);
                }

                // If we still assume to be consistent check more details
                if (!needRecreate)
                {
                    if (!tableType.Replace(SqlNotNull, string.Empty).Equals(pragmaInfo[i][2] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }

                    if (!tableColumn.Equals(pragmaInfo[i][1] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }
                }

                columnSegments.Add(string.Format("{0} {1}", tableColumn, tableType));
            }

            if (primaryKeySegments.Count > 0)
            {
                columnSegments.Add(string.Format("PRIMARY KEY ({0})", string.Join(",", primaryKeySegments)));
            }

            if (needRecreate && pragmaInfo.Count > 0)
            {
                throw new NotImplementedException("Table was inconsistent but re-creation is not yet supported!");
            }

            if (needRecreate)
            {
                this.log.Debug("Table {0} needs to be re-created", tableName);
                command = this.connection.CreateCommand();
                command.CommandText = string.Format("CREATE TABLE {0}({1})", tableName, string.Join(",", columnSegments));
                command.ExecuteNonQuery();
            }

            this.checkedTableList.Add(tableName);
        }

        private string GetTableType(Type internalType)
        {
            string arguments = string.Empty;
            if (internalType.IsGenericType && internalType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                internalType = internalType.GetGenericArguments()[0];
            }
            else
            {
                if (!internalType.IsClass)
                {
                    arguments = " NOT NULL";
                }
            }

            if (internalType == typeof(string))
            {
                return string.Concat("VARCHAR", arguments);
            }

            if (internalType == typeof(int)
                || internalType == typeof(uint)
                || internalType == typeof(long)
                || internalType == typeof(ulong)
                || internalType == typeof(bool)
                || internalType == typeof(ResourceLink))
            {
                return string.Concat("INTEGER", arguments);
            }

            if (internalType == typeof(float))
            {
                return string.Concat("FLOAT", arguments);
            }

            // SQLite does not have DateTime, instead we store as text and use conversion functions
            if (internalType == typeof(DateTime))
            {
                return string.Concat("TEXT", arguments);
            }

            throw new NotImplementedException("Type for value is not implemented: " + internalType);
        }
    }
}
