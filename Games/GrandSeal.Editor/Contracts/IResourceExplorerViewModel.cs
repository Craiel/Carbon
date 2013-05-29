using System.Collections.Generic;
using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    public interface IResourceExplorerViewModel : IEditorTool
    {
        IReadOnlyCollection<IFolderViewModel> Folders { get; }

        ICommand CommandSave { get; }
        ICommand CommandRefresh { get; }
        ICommand CommandAddFolder { get; }
        ICommand CommandReload { get; }
    }
}
