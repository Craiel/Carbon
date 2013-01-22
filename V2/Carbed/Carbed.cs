using System.Windows;

using Carbed.Contracts;
using Carbed.Views;

using Carbon.Engine.Contracts;

using Core.Utils.Contracts;

namespace Carbed
{
    public class Carbed : ICarbed
    {
        private readonly IEngineFactory factory;
        private readonly IEventRelay eventRelay;

        private readonly Application application;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Carbed(IEngineFactory factory)
        {
            this.factory = factory;
            this.eventRelay = factory.Get<IEventRelay>();
            
            this.application = new Application();
        }

        public MainView MainView { get; private set; }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Run()
        {
            var vm = this.factory.Get<IMainViewModel>();
            this.MainView = new MainView(this.eventRelay) { DataContext = vm };

            this.application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.application.MainWindow = this.MainView;
            this.application.Run(this.MainView);
        }
    }
}
