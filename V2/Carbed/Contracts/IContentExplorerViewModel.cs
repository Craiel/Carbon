using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IContentExplorerViewModel<T> : ICarbedTool
        where T : ICarbedDocument
    {
        ReadOnlyObservableCollection<T> Documents { get; }

        ICommand CommandReload { get; }
        ICommand CommandOpenNewDialog { get; }
    }
}
