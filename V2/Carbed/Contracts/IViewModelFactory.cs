namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel(ResourceTree data);

        IFontViewModel GetFontViewModel(FontEntry data);
        IStageViewModel GetStageViewModel(StageEntry data);
        IMaterialViewModel GetMaterialViewModel(MaterialEntry data);

        ITextureViewModel GetTextureViewModel(ResourceEntry data);
    }
}
