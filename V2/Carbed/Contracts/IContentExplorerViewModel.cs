using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IContentExplorerViewModel : ICarbedTool
    {
        ReadOnlyCollection<ICarbedDocument> Documents { get; }

        ICommand CommandReload { get; }
        ICommand CommandOpenNewDialog { get; }
    }
}
