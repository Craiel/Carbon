using System;
using System.Windows;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public abstract class ContentViewModel : DocumentViewModel
    {
        private readonly ICarbonContent data;
        
        private MetaDataEntry? name;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ContentViewModel(IEngineFactory factory, ICarbonContent data)
            : base(factory)
        {
            this.data = data;

        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HasName
        {
            get
            {
                return this.name != null;
            }
        }

        public override string Name
        {
            get
            {
                if (this.name == null)
                {
                    return "<no name>";
                }

                return this.name.Value.Value;
            }
            set
            {
                if (this.name == null && value == null)
                {
                    return;
                }

                if (value == null)
                {
                    // Todo: be sure to delete name entry...
                    this.name = null;
                    return;
                }
                
                if (this.name == null || this.name.Value.Value != value)
                {
                    this.CreateUndoState();

                    if (name == null)
                    {
                        name = new MetaDataEntry();
                    }
                    
                    ((MetaDataEntry)this.name).Value = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("HasName");
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnDelete(object arg)
        {
            if (MessageBox.Show(
                "Delete Content " + this.Name,
                "Confirmation",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.OnClose(null);
        }

        protected override object CreateMemento()
        {
            return this.data.Clone();
        }

        protected override void RestoreMemento(object memento)
        {
            ICarbonContent source = memento as ICarbonContent;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.CreateUndoState();
            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
