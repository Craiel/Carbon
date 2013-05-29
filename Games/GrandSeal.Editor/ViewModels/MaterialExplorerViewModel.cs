using System.Collections.ObjectModel;
using System.Collections.Specialized;

using GrandSeal.Editor.Contracts;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.ViewModels
{
    public class MaterialExplorerViewModel : ContentExplorerViewModel<IMaterialViewModel>, IMaterialExplorerViewModel
    {
        private readonly IEditorLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialExplorerViewModel(IEngineFactory factory, IEditorLogic logic)
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
