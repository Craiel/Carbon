using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;

namespace Carbon.Engine.Logic
{
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
        
        public virtual void Update(ITimer gameTime)
        {
        }

        public virtual void Unload()
        {
        }
    }
}
