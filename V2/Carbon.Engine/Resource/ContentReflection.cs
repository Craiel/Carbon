using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class ContentReflectionProperty
    {
        public ContentReflectionProperty(ContentEntryElementAttribute attribute, PropertyInfo info)
        {
            this.Name = attribute.Name ?? info.Name;
            this.Info = info;
        }

        public string Name { get; private set; }
        public PropertyInfo Info { get; private set; }
    }

    public static class ContentReflection
    {
        private static readonly IDictionary<Type, string> tableNameCache;
        private static readonly IDictionary<Type, IList<ContentReflectionProperty>> propertyLookupCache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static ContentReflection()
        {
            tableNameCache = new Dictionary<Type, string>();
            propertyLookupCache = new Dictionary<Type, IList<ContentReflectionProperty>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static string GetTableName(Type key)
        {
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

        public static string GetTableName<T>() where T : ICarbonContent
        {
            return GetTableName(typeof(T));
        }

        public static IList<ContentReflectionProperty> GetPropertyInfos(Type type)
        {
            if (!propertyLookupCache.ContainsKey(type))
            {
                BuildLookupCache(type);
            }
            
            return propertyLookupCache[type];
        }

        public static IList<ContentReflectionProperty> GetPropertyInfos<T>() where T : ICarbonContent
        {
            return GetPropertyInfos(typeof(T));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void BuildLookupCache(Type type)
        {
            IList<ContentReflectionProperty> properties = new List<ContentReflectionProperty>();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                var attribute = info.GetCustomAttributes(typeof(ContentEntryElementAttribute), false).FirstOrDefault() as
                    ContentEntryElementAttribute;
                if (attribute != null)
                {
                    properties.Add(new ContentReflectionProperty(attribute, info));
                }
            }

            propertyLookupCache.Add(type, properties);
        }
    }
}
