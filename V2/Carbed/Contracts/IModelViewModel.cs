using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IModelViewModel : IResourceViewModel
    {
        string FileName { get; }

        ICommand CommandSelectFile { get; }
    }
}
