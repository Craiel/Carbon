using Carbon.Editor.Resource;

namespace Carbed.Contracts
{
    public interface IViewModelFactory
    {
        IProjectFolderViewModel GetFolderViewModel(SourceProjectFolder data);
        ITextureFontViewModel GetTextureFontViewModel(SourceTextureFont data);
        IModelViewModel GetModelViewModel(SourceModel data);
    }
}
