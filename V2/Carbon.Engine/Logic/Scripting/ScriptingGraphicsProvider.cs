namespace Carbon.Engine.Logic.Scripting
{
    using Carbon.Engine.Contracts.Logic;

    public class ScriptingGraphicsProvider : IScriptingProvider
    {
        private readonly ICarbonGraphics graphics;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptingGraphicsProvider(ICarbonGraphics graphics)
        {
            this.graphics = graphics;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingProperty]
        public ScriptingGraphicsProvider Graphic
        {
            get
            {
                return this;
            }
        }

        public void ClearCache()
        {
            this.graphics.ClearCache();
        }
    }
}
