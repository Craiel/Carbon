﻿using System.Windows;

using Core.Engine.Contracts;
using Core.Utils.Contracts;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Views;

namespace GrandSeal.Editor
{
    public class Editor : IEditor
    {
        private readonly IEngineFactory factory;
        private readonly IEventRelay eventRelay;

        private readonly Application application;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Editor(IEngineFactory factory)
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
