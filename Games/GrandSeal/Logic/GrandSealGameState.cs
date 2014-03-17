namespace GrandSeal.Logic
{
    using CarbonCore.Utils.IO;

    using Contracts;

    using Core.Engine.Contracts;
    using Core.Engine.Logic;

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
