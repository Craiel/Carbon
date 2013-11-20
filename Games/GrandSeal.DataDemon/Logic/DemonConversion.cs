namespace GrandSeal.DataDemon.Logic
{
    using Core.Engine.Contracts;
    using Core.Utils.Contracts;

    using GrandSeal.DataDemon.Contracts;

    public class DemonConversion : DemonOperation, IDemonConversion
    {
        private readonly DemonConversionConfig config;
        private readonly ILog log;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DemonConversion(IEngineFactory factory, DemonConversionConfig config)
        {
            this.config = config;
            this.log = factory.Get<IDemonLog>().AquireContextLog("Conversion");
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
            this.log.Debug("Refreshing {0}", this.Name);
        }

        public override void Process()
        {
            if (this.EntriesToProcess <= 0)
            {
                return;
            }

            this.log.Debug("Processing {0} Entries", this.EntriesToProcess);
        }
    }
}
