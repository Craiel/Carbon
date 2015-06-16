namespace GrandSeal.Editor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    using GrandSeal.Editor.Contracts;

    public class MaterialExplorerViewModel : ContentExplorerViewModel<IMaterialViewModel>, IMaterialExplorerViewModel
    {
        private readonly IEditorLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialExplorerViewModel(IFactory factory, IEditorLogic logic)
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

            this.NotifyPropertyChangedExplicit("Title");
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
