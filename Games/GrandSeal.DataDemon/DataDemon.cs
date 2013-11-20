namespace GrandSeal.DataDemon
{
    using System.IO;
    using System.Threading;

    using Core.Engine.Contracts;
    using Core.Utils.Contracts;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;
    
    public class DataDemon : IDataDemon
    {
        private readonly IDemonLogic logic;
        private readonly ILog log;

        private DemonArguments arguments;
        private bool isRunning;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataDemon(IEngineFactory factory)
        {
            this.logic = factory.Get<IDemonLogic>();
            this.log = factory.Get<IDemonLog>().AquireContextLog("Demon");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Run(DemonArguments args)
        {
            this.arguments = args;
            this.log.Info("Demon starting up...");
            if (!this.PrepareEnvironment())
            {
                return;
            }

            this.log.Info("Entering loop");
            this.isRunning = true;
            while (this.isRunning)
            {
                this.logic.Refresh();
                Thread.Sleep(this.logic.RefreshInterval);
            }

            this.log.Info("Shutting down");
            this.logic.Dispose();
        }

        private bool PrepareEnvironment()
        {
            if (string.IsNullOrEmpty(this.arguments.Config) || !File.Exists(this.arguments.Config))
            {
                this.log.Error("No valid configuration specified!");
                return false;
            }

            if (!this.logic.LoadConfig(this.arguments.Config))
            {
                this.log.Error("Failed to load demon configuration!");
                return false;
            }

            return true;
        }
    }
}
