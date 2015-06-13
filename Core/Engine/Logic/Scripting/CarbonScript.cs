namespace Core.Engine.Logic.Scripting
{
    using CarbonCore.Processing.Resource;
    
    public class CarbonScript
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonScript()
        {
        }

        public CarbonScript(ScriptResource resource)
        {
            this.Script = resource.Script;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Script { get; set; }
    }
}
