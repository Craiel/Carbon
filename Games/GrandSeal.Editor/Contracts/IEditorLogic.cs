﻿using System.Collections.Generic;

using Core.Engine.Logic;
using Core.Engine.Resource.Content;

namespace GrandSeal.Editor.Contracts
{
    using System.Collections.ObjectModel;

    public delegate void ProjectChangedEventHandler();

    public interface IEditorLogic : IEditorBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        bool IsProjectLoaded { get; }

        ReadOnlyObservableCollection<IMaterialViewModel> Materials { get; }
        ReadOnlyObservableCollection<IFontViewModel> Fonts { get; }
        ReadOnlyObservableCollection<IFolderViewModel> Folders { get; }
        ReadOnlyObservableCollection<CarbonPath> RecentProjects { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(CarbonPath file);
        void SaveProject(CarbonPath file);

        void Reload();

        IMaterialViewModel AddMaterial();
        void Save(IMaterialViewModel material);
        void Delete(IMaterialViewModel material);
        IMaterialViewModel Clone(IMaterialViewModel source);

        IFontViewModel AddFont();
        void Save(IFontViewModel material);
        void Delete(IFontViewModel material);
        IFontViewModel Clone(IFontViewModel source);

        IFolderViewModel AddFolder();
        void Save(IFolderViewModel folder, bool force = false);
        void Delete(IFolderViewModel folder);
        IFolderViewModel Clone(IFolderViewModel source);

        IResourceTextureViewModel AddResourceTexture();
        IResourceModelViewModel AddResourceModel();
        IResourceScriptViewModel AddResourceScript();
        IResourceRawViewModel AddResourceRaw();
        IResourceFontViewModel AddResourceFont();
        IResourceStageViewModel AddResourceStage();
        IResourceUserInterfaceViewModel AddResourceUserInterface();

        void Save(IResourceViewModel resource);
        void Delete(IResourceViewModel resource);
        IResourceViewModel Clone(IResourceViewModel source);

        IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue, MetaDataTargetEnum target);
        IList<IFolderViewModel> GetResourceTreeChildren(int parent);
        IList<IResourceViewModel> GetResourceTreeContent(int node);

        IFolderViewModel LocateFolder(string hash);

        // Todo: Need to optimize and refactor this:
        IResourceViewModel LocateResource(int id);
        IResourceViewModel LocateResource(string hash);

        IList<IResourceViewModel> LocateResources(string filter);

        void ReloadSettings();
        void SaveSettings();
    }
}
