namespace Core.Engine.IoC
{
    using Autofac;

    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Resource;

    public class EngineFactory : IEngineFactory
    {
        private readonly IContainer kernel;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineFactory(IContainer kernel)
        {
            this.kernel = kernel;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IContainer Kernel
        {
            get
            {
                return this.kernel;
            }
        }

        public T Get<T>()
        {
            return this.kernel.Resolve<T>();
        }

        public ICarbonGraphics GetGraphics(IResourceManager resourceManager)
        {
            return this.kernel.Resolve<ICarbonGraphics>(new NamedParameter("resourceManager", resourceManager));
        }

        public IResourceManager GetResourceManager(CarbonPath root)
        {
            return this.kernel.Resolve<IResourceManager>(new NamedParameter("root", root));
        }

        public IContentManager GetContentManager(IResourceManager resourceManager, CarbonFile file)
        {
            return this.kernel.Resolve<IContentManager>(new NamedParameter("resourceManager", resourceManager), new NamedParameter("file", file));
        }
    }
}
