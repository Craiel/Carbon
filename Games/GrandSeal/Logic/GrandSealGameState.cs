using Core.Engine.Contracts;
using Core.Engine.Logic;
using GrandSeal.Contracts;

namespace GrandSeal.Logic
{
    public class GrandSealGameState : GameState, IGrandSealGameState
    {
        public GrandSealGameState(IEngineFactory factory)
            : base(factory)
        {
            this.ResourceManager = factory.GetResourceManager("Data");
            this.ContentManager = factory.GetContentManager(this.ResourceManager, "Main.db");
        }
    }
}
