namespace GrandSeal.Editor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using CarbonCore.Utils.Contracts.IoC;

    using GrandSeal.Editor.Contracts;

    public class FontExplorerViewModel : ContentExplorerViewModel<IFontViewModel>, IFontExplorerViewModel
    {
        private readonly IEditorLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FontExplorerViewModel(IFactory factory, IEditorLogic logic)
            : base(factory, logic)
        {
            this.logic = logic;
            ((INotifyCollectionChanged)this.logic.Fonts).CollectionChanged += this.OnSourceCollectionChangend;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return string.Format("Fonts {0} / {1}", this.Documents.Count, this.logic.Fonts.Count);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoUpdate(ObservableCollection<IFontViewModel> target)
        {
            foreach (IFontViewModel font in this.logic.Fonts)
            {
                target.Add(font);
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
