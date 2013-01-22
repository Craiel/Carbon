using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IModelViewModel : ICarbedDocument, IProjectFolderContent
    {
        bool HasName { get; }

        string FileName { get; }

        ICommand CommandSelectFile { get; }
    }
}
