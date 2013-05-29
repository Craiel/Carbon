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

        public CarbonScript(RawResource resource)
        {
            this.Script = System.Text.Encoding.ASCII.GetString(resource.Data);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Script { get; set; }
    }
}
