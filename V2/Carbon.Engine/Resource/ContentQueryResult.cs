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
        private readonly string statement;

        private IList<object[]> results;

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
        public int Count
        {
            get
            {
                this.EvaluateCommand();
                return this.results.Count;
            }
        }

        public IList ToList(Type type)
        {
            this.EvaluateCommand();
            return this.ProcessResults(type);
        }

        public IList<T> ToList<T>()
        {
            this.EvaluateCommand();
            return this.ProcessResults(typeof(T)).Cast<T>().ToList();
        }

        private void EvaluateCommand()
        {
            if (this.results != null)
            {
                return;
            }

            this.results = new List<object[]>();
            using (DbDataReader reader = this.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    object[] buffer = new object[reader.FieldCount];
                    if (reader.GetValues(buffer) != reader.FieldCount)
                    {
                        throw new InvalidOperationException("GetValues returned wrong number of values, expected " + reader.FieldCount);
                    }

                    this.results.Add(buffer);
                }
            }
        }

        private IList ProcessResults(Type targetType)
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            IList processed = new List<object>();


            return processed;
        }

        /*var stopWatch = new Stopwatch();
            stopWatch.Start();
            IList result;
            using (DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                result = this.LoadFromReader(targetType, reader);
            }

            stopWatch.Stop();
            Debug.WriteLine("DirectDataQuery: {0} executed in {1}ms", targetType, stopWatch.ElapsedMilliseconds);

            if (this.LookupDataLoaded != null)
            {
                this.LookupDataLoaded(this, new LookupDataLoadedEventArgs(targetType, result.Count, stopWatch.ElapsedMilliseconds));
            }

            return result;*/


        /*List<LookupData> results = new List<LookupData>();
            LookupColumnInfo[] columnInfos = LookupDataReflection.GetColumnInfo(targetType).ToArray();
            object[] temp = new object[columnInfos.Length];

            while (reader.Read())
            {
                if (reader.GetValues(temp) != columnInfos.Length)
                {
                    throw new DataException("Read count does not match expected columns");
                }

                LookupData translated = Activator.CreateInstance(targetType) as LookupData;
                for (int i = 0; i < temp.Length; i++)
                {
                    // Skip all null and DBNull sets, default for all object and nullables
                    if (temp[i] == null || temp[i] is DBNull)
                    {
                        continue;
                    }

                    object value = TypeConversion.GetTypedValue(columnInfos[i].Type, temp[i]);
                    columnInfos[i].PropertyInfo.SetValue(translated, value, null);
                }

                results.Add(translated);
            }

            return results;*/
    }
}
