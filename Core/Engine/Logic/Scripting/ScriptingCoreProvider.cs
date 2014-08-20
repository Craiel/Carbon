namespace Core.Engine.Logic.Scripting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts;

    using Core.Engine.Contracts.Logic;

    using NLua;

    using SharpDX;

    public class ScriptingCoreProvider : IScriptingProvider
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingMethod]
        public static object[] Params(LuaTable table)
        {
            if (table == null)
            {
                return null;
            }

            var entries = new List<object>();
            foreach (DictionaryEntry entry in table)
            {
                entries.Add(entry.Value);
            }

            return entries.ToArray();
        }

        [ScriptingMethod]
        public static string Desc(object source)
        {
            Type type = source.GetType();
            return type.Name;
        }

        [ScriptingMethod]
        public static Vector2 Vector2(float x = 0, float y = 0)
        {
            return new Vector2(x, y);
        }

        [ScriptingMethod]
        public static Vector3 Vector3(float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(x, y, z);
        }

        [ScriptingMethod]
        public static Vector4 Vector4(float x = 0, float y = 0, float z = 0, float w = 0)
        {
            return new Vector4(x, y, z, w);
        }
    }
}
