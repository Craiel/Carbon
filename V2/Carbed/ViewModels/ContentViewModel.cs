using System;
using System.Collections.Generic;
using System.Windows;

using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    using Core.Utils.Contracts;

    public abstract class ContentViewModel : DocumentViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly ICarbonContent data;

        private readonly ILog log;
        
        private MetaDataEntry name;

        private bool isLoaded;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ContentViewModel(IEngineFactory factory, ICarbonContent data)
            : base(factory)
        {
            this.logic = factory.Get<ICarbedLogic>();
            this.log = factory.Get<ICarbedLog>().AquireContextLog("ContentViewModel");
            this.data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name
        {
            get
            {
                if (this.name == null)
                {
                    return "<no name>";
                }

                return this.name.Value;
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
                
                if (this.name == null || this.name.Value != value)
                {
                    this.CreateUndoState();

                    if (name == null)
                    {
                        name = new MetaDataEntry { Key = MetaDataKey.Name };
                    }

                    this.name.Value = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("HasName");
                }
            }
        }

        public override void Load()
        {
            if (this.isLoaded)
            {
                this.log.Warning("Load called with entry already loaded");
                return;
            }

            base.Load();

            // Get the primary key information
            ContentReflectionProperty primaryKeyInfo = ContentReflection.GetPrimaryKeyPropertyInfo(this.data.GetType());

            // Load the metadata for this object
            IList<MetaDataEntry> metaData = this.logic.GetEntryMetaData(primaryKeyInfo.Info.GetValue(this.data));
            for (int i = 0; i < metaData.Count; i++)
            {
                if (metaData[i].Key == MetaDataKey.Name)
                {
                    this.name = metaData[i];
                    break;
                }
            }

            this.LoadMetadata(metaData);

            this.isLoaded = true;
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
            return this.data.Clone(fullCopy:true);
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

        protected void Save(IContentManager target)
        {
            if (this.name == null)
            {
                throw new InvalidOperationException("Can not save without setting a name first");
            }

            target.Save(ref this.name);
            this.NotifyPropertyChanged();
        }

        protected virtual void LoadMetadata(IList<MetaDataEntry> metaData)
        {
        }
    }
}
