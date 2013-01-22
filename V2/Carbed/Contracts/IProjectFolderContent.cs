using Carbon.Editor.Resource;

namespace Carbed.Contracts
{
    public interface IProjectFolderContent : ICarbedBase
    {
        bool IsSelected { get; set; }
        
        SourceFolderContent Data { get; }

        IProjectFolderViewModel Parent { get; set; }
    }
}
