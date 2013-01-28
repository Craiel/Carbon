using System;
using System.Collections.Generic;
using System.Reflection;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public static class ContentReflection
    {
        private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> propertyLookupCache;

        static ContentReflection()
        {
            propertyLookupCache = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        }

        public static PropertyInfo GetPropertyInfo<T>(string propertyName) where T : ICarbonContent
        {
            Type type = typeof(T);

            if (!propertyLookupCache.ContainsKey(type))
            {
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
