using Ninject;

namespace Core.Engine.Contracts
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Resource;
    using Core.Utils.IO;

    public interface IEngineFactory
    {
        IKernel Kernel { get; }

        T Get<T>();

        ICarbonGraphics GetGraphics(IResourceManager resourceManager);
        IResourceManager GetResourceManager(CarbonPath path);
        IContentManager GetContentManager(IResourceManager resourceManager, CarbonFile file);
    }
}
