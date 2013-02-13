namespace Carbed.Contracts
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using global::Carbed.ViewModels;

    public interface IContentExplorerViewModel : ICarbedTool
    {
        ExplorableContent SelectedContentType { get; set; }

        ReadOnlyCollection<ICarbedDocument> Documents { get; }

        ICommand CommandAdd { get; }
        ICommand CommandReload { get; }
    }
}
