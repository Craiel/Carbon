using System;
using System.Collections.Generic;
using System.Windows;

using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

using Core.Utils.Contracts;

namespace Carbed.ViewModels
{
    using System.Linq;

    public abstract class ContentViewModel : DocumentViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly ICarbonContent data;

        private readonly ILog log;

        private readonly IDictionary<MetaDataKey, MetaDataEntry> metaData;
        private readonly IList<MetaDataEntry> metaDataToDelete; 

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

            this.metaData = new Dictionary<MetaDataKey, MetaDataEntry>();
            this.metaDataToDelete = new List<MetaDataEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool IsNamed
        {
            get
            {
                string name = this.GetMetaValue(MetaDataKey.Name);
                return !string.IsNullOrEmpty(name);
            }
        }

        public override bool IsChanged
        {
            get
            {
                return this.data.IsChanged;
            }
        }

        public override string Name
        {
            get
            {
                string name = this.GetMetaValue(MetaDataKey.Name);
                if (string.IsNullOrEmpty(name))
                {
                    return "<no name>";
                }

                return name;
            }

            set
            {
                if (this.GetMetaValue(MetaDataKey.Name) != value)
                {
                    this.CreateUndoState();

                    this.SetMetaValue(MetaDataKey.Name, value);
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("IsChanged");
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
            this.metaData.Clear();
            IList<MetaDataEntry> metaDataList = this.logic.GetEntryMetaData(primaryKeyInfo.Info.GetValue(this.data), this.data.MetaDataTarget);
            for (int i = 0; i < metaDataList.Count; i++)
            {
                if (this.metaData.ContainsKey(metaDataList[i].Key))
                {
                    this.log.Warning("Duplicate metadata entry detected, will be removed on next save: {0}", metaDataList[i].Key);
                    this.metaDataToDelete.Add(metaDataList[i]);
                    continue;
                }

                this.metaData.Add(metaDataList[i].Key, metaDataList[i]);
            }
            
            this.isLoaded = true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected ILog Log
        {
            get
            {
                return this.log;
            }
        }

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
            var source = memento as ICarbonContent;
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
            if (!this.IsNamed)
            {
                throw new InvalidOperationException("Can not save without setting a name first");
            }

            if (this.data.IsChanged || this.data.IsNew)
            {
                target.Save(this.data);
            }

            foreach (MetaDataEntry entry in this.metaDataToDelete)
            {
                target.Delete(entry);
            }

            this.metaDataToDelete.Clear();

            var dataKey = (int?)ContentReflection.GetPrimaryKeyPropertyInfo(this.data.GetType()).Info.GetValue(this.data);
            foreach (KeyValuePair<MetaDataKey, MetaDataEntry> metaDataEntry in this.metaData)
            {
                if (metaDataEntry.Value != null && (metaDataEntry.Value.IsNew || metaDataEntry.Value.IsChanged))
                {
                    metaDataEntry.Value.TargetId = dataKey;
                    target.Save(metaDataEntry.Value);
                }
            }
        }

        protected void Delete(IContentManager target)
        {
            if (!this.data.IsNew)
            {
                target.Delete(this.data);
            }

            IList<MetaDataEntry> deleteQueue = this.metaDataToDelete.ToList();
            foreach (KeyValuePair<MetaDataKey, MetaDataEntry> metaDataEntry in this.metaData)
            {
                if (metaDataEntry.Value == null)
                {
                    continue;
                }

                deleteQueue.Add(metaDataEntry.Value);
            }

            foreach (MetaDataEntry entry in deleteQueue)
            {
                if (entry.IsNew)
                {
                    continue;
                }

                target.Delete(entry);
            }

            this.metaDataToDelete.Clear();
            this.metaData.Clear();
        }

        protected string GetMetaValue(MetaDataKey key)
        {
            if (this.metaData.ContainsKey(key) && this.metaData[key] != null)
            {
                return this.metaData[key].Value;
            }

            return null;
        }

        protected int? GetMetaValueInt(MetaDataKey key)
        {
            if (this.metaData.ContainsKey(key) && this.metaData[key] != null)
            {
                return this.metaData[key].ValueInt;
            }

            return null;
        }
        
        protected long? GetMetaValueLong(MetaDataKey key)
        {
            if (this.metaData.ContainsKey(key) && this.metaData[key] != null)
            {
                return this.metaData[key].ValueLong;
            }

            return null;
        }

        protected bool? GetMetaValueBit(MetaDataKey key, int bitNumber)
        {
            if (this.metaData.ContainsKey(key) && this.metaData[key] != null)
            {
                long bitValue = 1 << bitNumber;
                return (this.metaData[key].ValueLong & bitValue) != 0;
            }

            return null;
        }

        protected void SetMetaValue(MetaDataKey key, string value)
        {
            if (!this.metaData.ContainsKey(key))
            {
                this.metaData[key] = new MetaDataEntry { Key = key, Target = this.data.MetaDataTarget };
            }

            this.metaData[key].Value = value;
        }

        protected void SetMetaValue(MetaDataKey key, int? value)
        {
            if (!this.metaData.ContainsKey(key))
            {
                this.metaData[key] = new MetaDataEntry { Key = key, Target = this.data.MetaDataTarget };
            }

            this.metaData[key].ValueInt = value;
        }

        protected void SetMetaValue(MetaDataKey key, long? value)
        {
            if (!this.metaData.ContainsKey(key))
            {
                this.metaData[key] = new MetaDataEntry { Key = key, Target = this.data.MetaDataTarget };
            }

            this.metaData[key].ValueLong = value;
        }

        protected void SetMetaBitValue(MetaDataKey key, int bitNumber, bool value)
        {
            if (!this.metaData.ContainsKey(key))
            {
                this.metaData[key] = new MetaDataEntry { Key = key, Target = this.data.MetaDataTarget, ValueLong = 0 };
            }

            if (this.metaData[key].ValueLong == null)
            {
                this.metaData[key].ValueLong = 0;
            }

            long bitValue = 1 << bitNumber;
            if (value)
            {
                this.metaData[key].ValueLong |= bitValue;
            }
            else
            {
                this.metaData[key].ValueLong ^= bitValue;
            }
        }
    }
}
