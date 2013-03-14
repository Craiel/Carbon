﻿namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel(ResourceTree data);

        IStageViewModel GetStageViewModel(StageEntry data);
        IMaterialViewModel GetMaterialViewModel(MaterialEntry data);

        IResourceViewModel GetResourceViewModel(ResourceEntry data);
    }
}
