using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;

using Ninject;

namespace Carbon.Engine.Contracts
{
    public interface IEngineFactory
    {
        IKernel Kernel { get; }

        T Get<T>();

        IContentManager GetContentManager(ResourceLink root);
    }
}
