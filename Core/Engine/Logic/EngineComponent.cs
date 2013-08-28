namespace Core.Engine.Logic
{
    using System.Diagnostics.CodeAnalysis;

    using Core.Engine.Contracts.Logic;
    using Core.Utils.Contracts;
    using Core.Utils.Threading;

    public abstract class ThreadableEngineComponent : ThreadQueuedComponent, IEngineComponent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Dispose()
        {
            this.Unload();
        }

        public virtual void Initialize(ICarbonGraphics graphics)
        {
        }

        public virtual void Unload()
        {
        }

        public virtual bool Update(ITimer gameTime)
        {
            return true;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class EngineComponent : IEngineComponent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Dispose()
        {
            this.Unload();
        }

        public virtual void Initialize(ICarbonGraphics graphics)
        {
        }

        public virtual void Unload()
        {
        }

        public virtual bool Update(ITimer gameTime)
        {
            return true;
        }
    }
}
