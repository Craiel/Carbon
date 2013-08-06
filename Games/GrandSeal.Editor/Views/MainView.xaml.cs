using System;
using System.ComponentModel;
using Core.Utils.Contracts;
using GrandSeal.Editor.Events;

namespace GrandSeal.Editor.Views
{
    using System.Windows;

    using Xceed.Wpf.AvalonDock.Layout.Serialization;

    public partial class MainView
    {
        private readonly IEventRelay eventRelay;
        
        public MainView(IEventRelay eventRelay)
        {
            this.eventRelay = eventRelay;
            this.eventRelay.Subscribe<EventLoadLayout>(this.OnLoadLayoutEvent);
            this.eventRelay.Subscribe<EventSaveLayout>(this.OnSaveLayoutEvent);

            InitializeComponent();

            this.Closing += this.OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            this.eventRelay.Relay(new EventWindowClosing());
        }

        private void OnLoadLayoutEvent(EventLoadLayout args)
        {
            // Not yet supported properly, can't do this
            if (args.File == null || args.File.IsNull || !args.File.Exists)
            {
                throw new ArgumentException();
            }

            var serializer = new XmlLayoutSerializer(this.DockingManager);
            using (var stream = args.File.OpenRead())
            {
                serializer.Deserialize(stream);
            }
        }

        private void OnSaveLayoutEvent(EventSaveLayout args)
        {
            var serializer = new XmlLayoutSerializer(this.DockingManager);
            using (var stream = args.File.OpenCreate())
            {
                serializer.Serialize(stream);
            }
        }

        private void OnRecentItemClick(object sender, RoutedEventArgs e)
        {
            this.Ribbon.SelectedTabIndex = 0;
        }
    }
}
