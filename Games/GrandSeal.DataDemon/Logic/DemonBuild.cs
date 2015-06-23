namespace GrandSeal.DataDemon.Logic
{
    using GrandSeal.DataDemon.Contracts;

    public class DemonBuild : DemonOperation, IDemonBuild
    {
        private DemonBuildConfig config;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        public override void Refresh()
        {
            System.Diagnostics.Trace.TraceInformation("Refreshing build {0}", this.Name);
        }

        public override void Process()
        {
            System.Diagnostics.Trace.TraceInformation("Processing build {0}", this.Name);
        }

        public void SetConfig(DemonBuildConfig build)
        {
            this.config = build;
        }
    }
}
