using Core.Engine.Contracts.Logic;

namespace Core.Engine.Logic.Scripting
{
    public class ScriptingGameProvider : IScriptingProvider
    {
        private readonly ICarbonGame game;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingGameProvider(ICarbonGame game)
        {
            this.game = game;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void ClearCache()
        {
            this.game.ClearCache();
        }
    }
}
