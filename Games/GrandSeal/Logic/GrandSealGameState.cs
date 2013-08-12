namespace GrandSeal.Logic
{
    using Contracts;

    using Core.Engine.Contracts;
    using Core.Engine.Logic;
    using Core.Utils.IO;

    public class GrandSealGameState : GameState, IGrandSealGameState
    {
        public GrandSealGameState(IEngineFactory factory)
            : base(factory)
        {
            this.ResourceManager = factory.GetResourceManager(new CarbonDirectory("Data"));
            this.ContentManager = factory.GetContentManager(this.ResourceManager, new CarbonFile("Main.db"));
        }
    }
}
