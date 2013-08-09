namespace Core.Engine.Logic
{
    using Core.Engine.Contracts.Logic;
    using Core.Utils.Contracts;

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
