using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    public interface IContentExplorerViewModel<T> : IEditorTool
        where T : IEditorDocument
    {
        ReadOnlyObservableCollection<T> Documents { get; }

        ICommand CommandSave { get; }
        ICommand CommandReload { get; }
        ICommand CommandOpenNewDialog { get; }
    }
}
