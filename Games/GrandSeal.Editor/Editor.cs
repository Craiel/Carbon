namespace GrandSeal.Editor
{
    using System.ComponentModel;
    using System.Windows;

    using CarbonCore.Utils.Contracts;

    using Core.Engine.Contracts;

    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Views;

    public class Editor : IEditor
    {
        private readonly IEngineFactory factory;
        private readonly IEventRelay eventRelay;
        private readonly IEditorLogic logic;

        private readonly Application application;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Editor(IEngineFactory factory)
        {
            this.factory = factory;
            this.eventRelay = factory.Get<IEventRelay>();
            this.logic = factory.Get<IEditorLogic>();
            
            this.application = new Application();

            this.logic.ReloadSettings();
        }

        public MainView MainView { get; private set; }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Run()
        {
            var vm = this.factory.Get<IMainViewModel>();
            this.MainView = new MainView(this.eventRelay) { DataContext = vm };
            this.MainView.Closing += this.OnMainViewClosing;

            this.application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.application.MainWindow = this.MainView;
            this.application.Run(this.MainView);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnMainViewClosing(object sender, CancelEventArgs args)
        {
            // Todo: Add checks if we have unsaved changes and save them
            this.logic.SaveSettings();
        }
    }
}
