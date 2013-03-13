using System;

using Carbon.Engine.Contracts.Logic;

using Core.Utils.Contracts;

using SlimDX;

namespace Carbon.Engine.Logic.Scripting
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
        public string Desc(object source)
        {
            Type type = source.GetType();
            return type.Name;
        }

        [ScriptingMethod]
        public Vector2 Vector2(float x = 0, float y = 0)
        {
            return new Vector2(x, y);
        }

        [ScriptingMethod]
        public Vector3 Vector3(float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(x, y, z);
        }

        [ScriptingMethod]
        public Vector4 Vector4(float x = 0, float y = 0, float z = 0, float w = 0)
        {
            return new Vector4(x, y, z, w);
        }
    }
}
