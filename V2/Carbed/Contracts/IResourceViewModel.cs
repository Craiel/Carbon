using System.Windows.Input;

using Carbon.Engine.Contracts.Resource;

namespace Carbed.Contracts
{
    public interface IResourceViewModel : ICarbedDocument
    {
        IFolderViewModel Parent { get; set; }

        ICommand CommandSelectFile { get; }

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
