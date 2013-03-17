﻿namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel(ResourceTree data);

        IStageViewModel GetStageViewModel(StageEntry data);
        IMaterialViewModel GetMaterialViewModel(MaterialEntry data);

        IResourceTextureViewModel GetResourceTextureViewModel(ResourceEntry data);
        IResourceModelViewModel GetResourceModelViewModel(ResourceEntry data);
        IResourceScriptViewModel GetResourceScriptViewModel(ResourceEntry data);
        IResourceRawViewModel GetResourceRawViewModel(ResourceEntry data);
    }
}
