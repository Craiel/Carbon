using Carbon.Editor.Resource;

namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IProjectFolderViewModel GetFolderViewModel(SourceProjectFolder data);

        IFontViewModel GetFontViewModel(FontEntry data);
        IPlayfieldViewModel GetPlayfieldViewModel(PlayfieldEntry data);

        IModelViewModel GetModelViewModel(SourceModel data);
    }
}
