using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Logic.Scripting;
using Carbon.V2Test.Contracts;

using Core.Utils;

namespace Carbon.V2Test.Logic
{
    public class V2TestGameState : GameState, IV2TestGameState
    {
        public V2TestGameState(IEngineFactory factory)
        {
            this.ResourceManager = factory.GetResourceManager("Data");
            this.ContentManager = factory.GetContentManager(this.ResourceManager, "Main.db");

            this.ScriptingEngine = factory.Get<IScriptingEngine>();
            this.ScriptingEngine.Register(new ScriptingCoreProvider(factory.Get<IApplicationLog>()));
            this.ScriptingEngine.Register(factory.Get<IInputManager>());

            
        }

        public override void Dispose()
        {
            base.Dispose();

            this.ResourceManager.Dispose();
            this.ContentManager.Dispose();
            this.SceneManager.Dispose();
        }
    }
}
