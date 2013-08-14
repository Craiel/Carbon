using Core.Engine.Resource.Content;
using Core.Utils.IO;
using System.Collections.Generic;

namespace GrandSeal.Editor.Contracts
{
    using System.Collections.ObjectModel;

    public delegate void ProjectChangedEventHandler();

    public interface IEditorLogic : IEditorBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        bool IsProjectLoaded { get; }

        CarbonDirectory ProjectLocation { get; }

        ReadOnlyObservableCollection<IMaterialViewModel> Materials { get; }
        ReadOnlyObservableCollection<IFontViewModel> Fonts { get; }
        ReadOnlyObservableCollection<IFolderViewModel> Folders { get; }
        ReadOnlyObservableCollection<CarbonDirectory> RecentProjects { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(CarbonDirectory path);
        void SaveProject(CarbonDirectory path);

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

        IResourceViewModel LocateResource(int id);
        IResourceViewModel LocateResource(string hash);
        IResourceViewModel LocateResource(CarbonFile file);

        IList<IResourceViewModel> LocateResources(string filter);

        void ReloadSettings();
        void SaveSettings();
    }
}
