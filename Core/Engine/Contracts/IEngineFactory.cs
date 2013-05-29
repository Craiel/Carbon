using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Resource;

using Ninject;

namespace Core.Engine.Contracts
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
