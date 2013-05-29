using System.Reflection;

namespace Core.Engine.Logic.Scripting
{
    public class ScriptingPropertyInfo
    {
        public ScriptingPropertyInfo(PropertyInfo propertyInfo)
        {
            this.Name = propertyInfo.Name;
            this.PropertyInfo = propertyInfo;
        }

        public string Name { get; set;}
        public PropertyInfo PropertyInfo { get; private set; }
    }
}
