using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Carbed.Contracts;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public class FontExplorerViewModel : ContentExplorerViewModel<IFontViewModel>, IFontExplorerViewModel
    {
        private readonly ICarbedLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FontExplorerViewModel(IEngineFactory factory, ICarbedLogic logic)
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
