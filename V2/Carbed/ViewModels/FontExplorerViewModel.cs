using Carbed.Contracts;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

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

        private void OnSourceCollectionChangend(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateDocuments();
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
        }
    }
}
