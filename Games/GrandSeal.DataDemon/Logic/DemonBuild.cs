namespace GrandSeal.DataDemon.Logic
{
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;

    using GrandSeal.DataDemon.Contracts;

    public class DemonBuild : DemonOperation, IDemonBuild
    {
        private readonly ILog log;

        private DemonBuildConfig config;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DemonBuild(IFactory factory)
        {
            this.log = factory.Resolve<IDemonLog>().AquireContextLog("Build");
        }

        public override string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        public override void Refresh()
        {
            this.log.Debug("Refreshing build {0}", this.Name);
        }

        public override void Process()
        {
            this.log.Debug("Processing build {0}", this.Name);
        }

        public void SetConfig(DemonBuildConfig build)
        {
            this.config = build;
        }
    }
}
