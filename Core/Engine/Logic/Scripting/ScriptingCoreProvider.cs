﻿using System;
using System.Collections;
using System.Collections.Generic;

using Core.Engine.Contracts.Logic;

using Core.Utils.Contracts;

using SlimDX;

namespace Core.Engine.Logic.Scripting
{
    public class ScriptingCoreProvider : IScriptingProvider
    {
        private readonly ILog log;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingCoreProvider(ILogBase log)
        {
            this.log = log.AquireContextLog("CoreProvider");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingProperty]
        public ILog Log
        {
            get
            {
                return this.log;
            }
        }

        [ScriptingMethod]
        public static object[] Params(LuaInterface.LuaTable table)
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