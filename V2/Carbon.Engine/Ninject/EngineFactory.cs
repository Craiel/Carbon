using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;

using Ninject;
using Ninject.Parameters;

namespace Carbon.Engine.Ninject
{
    public class EngineFactory : IEngineFactory
    {
        private readonly IKernel kernel;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IKernel Kernel
        {
            get
            {
                return this.kernel;
            }
        }

        public T Get<T>()
        {
            return this.kernel.Get<T>();
        }

        public IResourceManager GetResourceManager(string root)
        {
            return this.kernel.Get<IResourceManager>(new ConstructorArgument("root", root));
        }

        public IContentManager GetContentManager(IResourceManager resourceManager, string root)
        {
            return this.kernel.Get<IContentManager>(
                new ConstructorArgument("resourceManager", resourceManager), new ConstructorArgument("root", root));
        }
    }
}
