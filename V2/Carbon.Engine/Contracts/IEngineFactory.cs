using Ninject;

namespace Carbon.Engine.Contracts
{
    public interface IEngineFactory
    {
        IKernel Kernel { get; }

        T Get<T>();
    }
}
