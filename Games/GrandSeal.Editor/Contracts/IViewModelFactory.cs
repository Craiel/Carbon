namespace GrandSeal.Editor.Contracts
{
    using Core.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel(ResourceTree data);

        IStageViewModel GetStageViewModel(StageEntry data);
        IMaterialViewModel GetMaterialViewModel(MaterialEntry data);
        IFontViewModel GetFontViewModel(FontEntry data);

        IResourceTextureViewModel GetResourceTextureViewModel(ResourceEntry data);
        IResourceModelViewModel GetResourceModelViewModel(ResourceEntry data);
        IResourceScriptViewModel GetResourceScriptViewModel(ResourceEntry data);
        IResourceRawViewModel GetResourceRawViewModel(ResourceEntry data);
        IResourceFontViewModel GetResourceFontViewModel(ResourceEntry data);
        IResourceStageViewModel GetResourceStageViewModel(ResourceEntry data);
    }
}
