namespace Core.Engine.Logic.Scripting
{
    using System.Reflection;

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
