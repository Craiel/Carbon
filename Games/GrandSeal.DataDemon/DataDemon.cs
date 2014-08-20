namespace GrandSeal.DataDemon
{
    using System.IO;
    using System.Threading;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;
    
    public class DataDemon : IDataDemon
    {
        private readonly IDemonLogic logic;

        private DemonArguments arguments;
        private bool isRunning;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataDemon(IFactory factory)
        {
            this.logic = factory.Resolve<IDemonLogic>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Run(DemonArguments args)
        {
            this.arguments = args;
            System.Diagnostics.Trace.TraceInformation("Demon starting up...");
            if (!this.PrepareEnvironment())
            {
                return;
            }

            System.Diagnostics.Trace.TraceInformation("Entering loop");
            this.isRunning = true;
            while (this.isRunning)
            {
                this.logic.Refresh();
                Thread.Sleep(this.logic.RefreshInterval);
            }

            System.Diagnostics.Trace.TraceInformation("Shutting down");
            this.logic.Dispose();
        }

        private bool PrepareEnvironment()
        {
            if (string.IsNullOrEmpty(this.arguments.Config) || !File.Exists(this.arguments.Config))
            {
                System.Diagnostics.Trace.TraceError("No valid configuration specified!");
                return false;
            }

            if (!this.logic.LoadConfig(this.arguments.Config))
            {
                System.Diagnostics.Trace.TraceError("Failed to load demon configuration!");
                return false;
            }

            return true;
        }
    }
}
