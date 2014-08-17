namespace GrandSeal.Logic
{
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    using Contracts;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Logic;

    public class GrandSealGameState : GameState, IGrandSealGameState
    {
        public GrandSealGameState(IFactory factory)
            : base(factory)
        {
            this.ResourceManager = factory.Resolve<IResourceManager>();
            this.ResourceManager.SetRoot(new CarbonDirectory("Data"));

            this.ContentManager = factory.Resolve<IContentManager>();
            this.ContentManager.Initialize(new CarbonFile("Main.db"));
        }
    }
}
