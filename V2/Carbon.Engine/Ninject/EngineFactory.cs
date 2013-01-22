using Carbon.Engine.Contracts;

using Ninject;

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
    }
}
