namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel();

        IFontViewModel GetFontViewModel(FontEntry data);
        IStageViewModel GetPlayfieldViewModel(PlayfieldEntry data);

        IResourceViewModel GetResourceViewModel(ResourceEntry data);
    }
}
