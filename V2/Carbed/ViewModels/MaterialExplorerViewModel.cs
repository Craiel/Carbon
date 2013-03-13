using Carbed.Contracts;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public class MaterialExplorerViewModel : ContentExplorerViewModel<IMaterialViewModel>, IMaterialExplorerViewModel
    {
        private readonly ICarbedLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialExplorerViewModel(IEngineFactory factory, ICarbedLogic logic)
            : base(factory, logic)
        {
            this.logic = logic;
            ((INotifyCollectionChanged)this.logic.Materials).CollectionChanged += this.OnSourceCollectionChangend;
        }

        private void OnSourceCollectionChangend(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateDocuments();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoUpdate(ObservableCollection<IMaterialViewModel> target)
        {
            foreach (IMaterialViewModel material in this.logic.Materials)
            {
                target.Add(material);
            }
        }
    }
}
