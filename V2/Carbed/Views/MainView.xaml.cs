﻿using System.ComponentModel;

using Carbed.Events;

using Core.Utils.Contracts;

namespace Carbed.Views
{
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
            /*if (string.IsNullOrEmpty(args.File) || !File.Exists(args.File))
            {
                throw new ArgumentException();
            }

            var serialzier = new XmlLayoutSerializer(this.dockingManager);
            using (var stream = new FileStream(args.File, FileMode.Open, FileAccess.Read))
            {
                serialzier.Deserialize(stream);
            }*/
        }

        private void OnSaveLayoutEvent(EventSaveLayout args)
        {
            /*var serializer = new XmlLayoutSerializer(this.dockingManager);
            using (var stream = new FileStream(args.File, FileMode.Create, FileAccess.ReadWrite))
            {
                serializer.Serialize(stream);
            }*/
        }
    }
}