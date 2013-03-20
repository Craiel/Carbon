using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Carbed.Contracts;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
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

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return string.Format("Materials {0} / {1}", this.Documents.Count, this.logic.Materials.Count);
            }
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

            this.NotifyPropertyChanged("Title");
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSourceCollectionChangend(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateDocuments();
        }
    }
}
