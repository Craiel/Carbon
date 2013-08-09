namespace Core.Engine.Logic.Scripting
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptingMethod : Attribute
    {
        public string Description { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here."), 
    AttributeUsage(AttributeTargets.Property)]
    public class ScriptingProperty : Attribute
    {
        public string Description { get; set; }
    }
}
