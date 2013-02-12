using Carbon.Engine.Contracts.Resource;

using Ninject;

namespace Carbon.Engine.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IEngineFactory
    {
        IKernel Kernel { get; }

        T Get<T>();

        IResourceManager GetResourceManager(string path);
        IContentManager GetContentManager(IResourceManager resourceManager, ResourceLink root);
    }
}
