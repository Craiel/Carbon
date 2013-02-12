using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IModelViewModel : ICarbedDocument, IProjectFolderContent
    {
        string FileName { get; }

        ICommand CommandSelectFile { get; }
    }
}
