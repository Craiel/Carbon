using System.Collections.Generic;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IResourceExplorerViewModel : ICarbedTool
    {
        IReadOnlyCollection<IFolderViewModel> Folders { get; }

        ICommand CommandSave { get; }
        ICommand CommandAddFolder { get; }
    }
}
