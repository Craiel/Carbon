using Carbon.Engine.Contracts;
using Carbon.Engine.Logic;
using Carbon.V2Test.Contracts;

namespace Carbon.V2Test.Logic
{
    public class V2TestGameState : GameState, IV2TestGameState
    {
        public V2TestGameState(IEngineFactory factory)
            : base(factory)
        {
            this.ResourceManager = factory.GetResourceManager("Data");
            this.ContentManager = factory.GetContentManager(this.ResourceManager, "Main.db");

            this.SceneEntityFactory = factory.GetEntityFactory(this.ResourceManager, this.ContentManager);
        }
    }
}
