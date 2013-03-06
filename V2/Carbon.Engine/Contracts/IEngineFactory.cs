using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;

using Ninject;

namespace Carbon.Engine.Contracts
{
    public interface IEngineFactory
    {
        IKernel Kernel { get; }

        T Get<T>();

        ICarbonGraphics GetGraphics(IResourceManager resourceManager);
        IResourceManager GetResourceManager(string path);
        IContentManager GetContentManager(IResourceManager resourceManager, string root);
    }
}
