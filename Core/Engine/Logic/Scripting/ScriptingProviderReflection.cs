namespace Core.Engine.Logic.Scripting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    using Core.Engine.Contracts.Logic;

    public static class ScriptingProviderReflection
    {
        private static readonly IDictionary<Type, IList<ScriptingMethodInfo>> MethodCache =
            new Dictionary<Type, IList<ScriptingMethodInfo>>();

        private static readonly IDictionary<Type, IList<ScriptingPropertyInfo>> PropertyCache =
            new ConcurrentDictionary<Type, IList<ScriptingPropertyInfo>>();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IList<ScriptingMethodInfo> GetMethods<T>() where T : IScriptingProvider
        {
            return DoGetMethods(typeof(T));
        }

        public static IList<ScriptingMethodInfo> GetMethods(Type type)
        {
            if (type.GetInterface(typeof(IScriptingProvider).Name) == null)
            {
                throw new ArgumentException("Type is not implementing provider interface");
            }

            return DoGetMethods(type);
        }

        public static IList<ScriptingPropertyInfo> GetProperties<T>() where T : IScriptingProvider
        {
            return DoGetProperties(typeof(T));
        }

        public static IList<ScriptingPropertyInfo> GetProperties(Type type)
        {
            if (type.GetInterface(typeof(IScriptingProvider).Name) == null)
            {
                throw new ArgumentException("Type is not implementing provider interface");
            }

            return DoGetProperties(type);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static IList<ScriptingMethodInfo> DoGetMethods(Type type)
        {
            if (!MethodCache.ContainsKey(type))
            {
                IList<ScriptingMethodInfo> methodInfos = new List<ScriptingMethodInfo>();
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    var attribute = method.GetCustomAttribute<ScriptingMethod>();
                    if (attribute != null)
                    {
                        var info = new ScriptingMethodInfo(method);
                        methodInfos.Add(info);
                    }
                }

                MethodCache.Add(type, methodInfos);
            }

            return MethodCache[type];
        }

        private static IList<ScriptingPropertyInfo> DoGetProperties(Type type)
        {
            if (!PropertyCache.ContainsKey(type))
            {
                IList<ScriptingPropertyInfo> propertyInfos = new List<ScriptingPropertyInfo>();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var attribute = property.GetCustomAttribute<ScriptingProperty>();
                    if (attribute != null)
                    {
                        var info = new ScriptingPropertyInfo(property);
                        propertyInfos.Add(info);
                    }
                }

                PropertyCache.Add(type, propertyInfos);
            }

            return PropertyCache[type];
        }
    }
}
