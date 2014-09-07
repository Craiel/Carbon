namespace Core.Engine.Logic
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Threading;

    using Core.Engine.Contracts.Logic;

    public abstract class ThreadableEngineComponent : ThreadQueuedComponent, IEngineComponent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.Unload();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class EngineComponent : IEngineComponent
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.Unload();
        }
    }
}
