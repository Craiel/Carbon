using Carbon.Project.Contracts;

namespace Carbon.Project.Data
{
    public class CarbonProject : ProjectData, ICarbonProject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public override void Save(System.IO.Stream target)
        {
        }
    }
}
