namespace GrandSeal.DataDemon.Logic
{
    using Core.Engine.Contracts;
    using Core.Utils.Contracts;

    using GrandSeal.DataDemon.Contracts;

    public class DemonBuild : DemonOperation, IDemonBuild
    {
        private readonly DemonBuildConfig config;
        private readonly ILog log;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DemonBuild(IEngineFactory factory, DemonBuildConfig config)
        {
            this.config = config;
            this.log = factory.Get<IDemonLog>().AquireContextLog("Build");
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
    }
}
