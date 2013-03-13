using System;

namespace Carbon.Engine.Logic.Scripting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptingMethod : Attribute
    {
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScriptingProperty : Attribute
    {
        public string Description { get; set; }
    }
}
