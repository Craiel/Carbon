using System.Reflection;

namespace Core.Engine.Logic.Scripting
{
    public class ScriptingMethodInfo
    {
        public ScriptingMethodInfo(MethodInfo method)
        {
            this.Name = method.Name;
            this.MethodInfo = method;
        }

        public string Name { get; set; }
        public MethodInfo MethodInfo { get; private set; }
    }
}
