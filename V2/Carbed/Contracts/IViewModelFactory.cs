namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel();

        IFontViewModel GetFontViewModel(FontEntry data);
        IPlayfieldViewModel GetPlayfieldViewModel(PlayfieldEntry data);

        IModelViewModel GetModelViewModel(ResourceEntry data);
    }
}
