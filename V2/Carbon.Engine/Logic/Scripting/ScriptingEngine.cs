using System;
using System.Collections.Generic;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;

using Core.Utils.Contracts;
using Core.Utils.Diagnostics;

using LuaInterface;

namespace Carbon.Engine.Logic.Scripting
{
    public class ScriptingEngine : IScriptingEngine
    {
        private readonly ILog log;
        private readonly IList<IScriptingProvider> providers;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingEngine(IEngineFactory factory)
        {
            this.log = factory.Get<IEngineLog>().AquireContextLog("ScriptingEngine");

            this.providers = new List<IScriptingProvider>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Register(IScriptingProvider provider)
        {
            if (this.providers.Contains(provider))
            {
                throw new ArgumentException("Provider was already registered");
            }

            this.providers.Add(provider);
        }

        public void Unregister(IScriptingProvider provider)
        {
            if (!this.providers.Contains(provider))
            {
                throw new ArgumentException("Provider was not registered");
            }

            this.providers.Remove(provider);
        }

        public void Execute(CarbonScript script)
        {
            if (script == null || string.IsNullOrEmpty(script.Script))
            {
                throw new ArgumentException("Execute was called with invalid script");
            }

            string processedScript = script.Script;
            try
            {
                using (new ProfileRegion("ScriptingEngine.Execute"))
                {
                    using (Lua runtime = new Lua())
                    {
                        this.PrepareRuntime(runtime);
                        runtime.DoString(processedScript);
                    }
                }
            }
            catch (Exception e)
            {
                this.log.Error("Error in script execution: {0} at {1}", e, e.Message, e.Source);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void PrepareRuntime(Lua runtime)
        {
            // Special function for enumerable collections
            runtime.DoString("function enum(o)\nif o == nil then return nil end\nlocal e = o:GetEnumerator()\nreturn function()\nif e:MoveNext() then return e.Current end end end");

            IList<IScriptingProvider> providerList = new List<IScriptingProvider>(this.providers);
            foreach (IScriptingProvider provider in providerList)
            {
                Type providerType = provider.GetType();
                IList<ScriptingMethodInfo> methods = ScriptingProviderReflection.GetMethods(providerType);
                IList<ScriptingPropertyInfo> properties = ScriptingProviderReflection.GetProperties(providerType);

                foreach (ScriptingMethodInfo method in methods)
                {
                    runtime.RegisterFunction(method.MethodInfo.Name, provider, method.MethodInfo);
                }

                foreach (ScriptingPropertyInfo property in properties)
                {
                    runtime[property.Name] = property.PropertyInfo.GetValue(provider);
                }
            }
        }
    }
}
