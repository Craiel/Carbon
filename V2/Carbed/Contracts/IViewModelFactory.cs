﻿namespace Carbed.Contracts
{
    using Carbon.Engine.Resource.Content;

    public interface IViewModelFactory
    {
        IFolderViewModel GetFolderViewModel();

        IFontViewModel GetFontViewModel(FontEntry data);
        IStageViewModel GetStageViewModel(StageEntry data);
        IMaterialViewModel GetMaterialViewModel(MaterialEntry data);

        IResourceViewModel GetResourceViewModel(ResourceEntry data);
    }
}
