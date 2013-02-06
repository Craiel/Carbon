using Carbon.Editor.Resource;

namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IProjectFolderViewModel GetFolderViewModel(SourceProjectFolder data);
        ITextureFontViewModel GetTextureFontViewModel(FontEntry data);
    }
}
