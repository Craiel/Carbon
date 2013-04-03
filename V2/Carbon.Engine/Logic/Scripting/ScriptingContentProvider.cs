using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Engine.Logic.Scripting
{
    using Carbon.Engine.Contracts.Logic;
    using Carbon.Engine.Contracts.Resource;

    public class ScriptingContentProvider : IScriptingProvider
    {
        private readonly IContentManager contentManager;
        private readonly IResourceManager resourceManager;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingContentProvider(IContentManager contentManager, IResourceManager resourceManager)
        {
            this.contentManager = contentManager;
            this.resourceManager = resourceManager;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingProperty]
        public ScriptingContentProvider Content
        {
            get
            {
                return this;
            }
        }

        public void ClearCache()
        {
            this.resourceManager.ClearCache();
            this.contentManager.ClearCache();
        }
    }
}
