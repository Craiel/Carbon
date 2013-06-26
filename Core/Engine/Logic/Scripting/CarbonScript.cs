using Core.Engine.Resource.Resources;

namespace Core.Engine.Logic.Scripting
{
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
