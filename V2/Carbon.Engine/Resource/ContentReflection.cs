using System;
using System.Collections.Generic;
using System.Reflection;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    using System.Linq;

    internal class ContentReflectionProperty
    {
        public ContentReflectionProperty(string name, PropertyInfo info)
        {
            this.Name = name;
            this.Info = info;
        }

        public string Name { get; private set; }
        public PropertyInfo Info { get; private set; }
    }

    public static class ContentReflection
    {
        private static readonly IDictionary<Type, string> tableNameCache;
        private static readonly IDictionary<Type, IList<ContentReflectionProperty>> propertyLookupCache;

        static ContentReflection()
        {
            tableNameCache = new Dictionary<Type, string>();
            propertyLookupCache = new Dictionary<Type, IList<ContentReflectionProperty>>();
        }

        public static string GetTableName<T>() where T : ICarbonContent
        {
            Type key = typeof(T);
            if (!tableNameCache.ContainsKey(key))
            {
                var attribute = key.GetCustomAttributes(typeof(ContentEntryAttribute), true).FirstOrDefault() as ContentEntryAttribute;
                if (attribute == null)
                {
                    throw new InvalidOperationException("Unknown error finding table specification");
                }

                tableNameCache.Add(key, attribute.Table);
            }

            return tableNameCache[key];
        }

        public static PropertyInfo GetPropertyInfo<T>(string propertyName) where T : ICarbonContent
        {
            Type type = typeof(T);

            if (!propertyLookupCache.ContainsKey(type))
            {
                // Todo: Build the entire cache for this type in one go
                propertyLookupCache.Add(type, new Dictionary<string, PropertyInfo>());
            }

            if (!propertyLookupCache[type].ContainsKey(propertyName))
            {
                PropertyInfo info = type.GetProperty(propertyName);
                if (info == null)
                {
                    throw new InvalidOperationException("Property was not found on type for criteria");
                }

                propertyLookupCache[type].Add(propertyName, info);
            }

            return propertyLookupCache[type][propertyName];
        }
    }
}
