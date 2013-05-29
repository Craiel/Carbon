using System;
using System.ComponentModel;
using System.IO;

using AvalonDock.Layout.Serialization;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Events;

using Core.Utils.Contracts;

namespace GrandSeal.Editor.Views
{
    public partial class MainView
    {
        private readonly IEventRelay eventRelay;

        private IMainViewModel currentSerializiationContext;

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
            if (string.IsNullOrEmpty(args.File) || !File.Exists(args.File))
            {
                throw new ArgumentException();
            }

            this.currentSerializiationContext = args.MainViewModel;
            var serializer = new XmlLayoutSerializer(this.dockingManager);
            using (var stream = new FileStream(args.File, FileMode.Open, FileAccess.Read))
            {
                serializer.Deserialize(stream);
            }

            this.currentSerializiationContext = null;
        }

        private void OnSaveLayoutEvent(EventSaveLayout args)
        {
            var serializer = new XmlLayoutSerializer(this.dockingManager);
            using (var stream = new FileStream(args.File, FileMode.Create, FileAccess.ReadWrite))
            {
                serializer.Serialize(stream);
            }
        }
    }
}
