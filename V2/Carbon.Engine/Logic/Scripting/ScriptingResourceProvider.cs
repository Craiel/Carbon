using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Resources;

using Core.Utils;

namespace Carbon.Engine.Logic.Scripting
{
    public class ScriptingResourceProvider : IScriptingProvider
    {
        private readonly IResourceManager resourceManager;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingResourceProvider(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingMethod]
        public ICarbonResource LoadRawResourceByFile(string file)
        {
            return this.LoadRawResource(HashUtils.BuildResourceHash(file));
        }

        [ScriptingMethod]
        public ICarbonResource LoadRawResource(string hash)
        {
            return this.resourceManager.Load<RawResource>(hash);
        }

        [ScriptingMethod]
        public ICarbonResource LoadModelByFile(string file)
        {
            return this.LoadModel(HashUtils.BuildResourceHash(file));
        }

        [ScriptingMethod]
        public ICarbonResource LoadModel(string hash)
        {
            return this.resourceManager.Load<ModelResource>(hash);
        }
    }
}
