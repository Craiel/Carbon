namespace Core.Engine.Contracts
{
    using Autofac;

    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Resource;

    public interface IEngineFactory
    {
        IContainer Kernel { get; }

        T Get<T>();

        ICarbonGraphics GetGraphics(IResourceManager resourceManager);
        IResourceManager GetResourceManager(CarbonPath path);
        IContentManager GetContentManager(IResourceManager resourceManager, CarbonFile file);
    }
}
