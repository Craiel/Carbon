using System.Collections.Generic;

using Carbon.Engine.Resource.Content;

namespace Carbed.Contracts
{
    using System.Collections.ObjectModel;

    public delegate void ProjectChangedEventHandler();

    public interface ICarbedLogic : ICarbedBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        bool IsProjectLoaded { get; }

        ReadOnlyObservableCollection<IMaterialViewModel> Materials { get; }
        ReadOnlyObservableCollection<IFolderViewModel> Folders { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(string file);
        void SaveProject(string file);

        void Reload();

        IMaterialViewModel AddMaterial();
        void Save(IMaterialViewModel material);
        void Delete(IMaterialViewModel material);
        IMaterialViewModel Clone(IMaterialViewModel source);

        IFolderViewModel AddFolder();
        void Save(IFolderViewModel folder);
        void Delete(IFolderViewModel folder);
        IFolderViewModel Clone(IFolderViewModel source);

        IResourceTextureViewModel AddResourceTexture();
        IResourceModelViewModel AddResourceModel();
        IResourceScriptViewModel AddResourceScript();
        IResourceRawViewModel AddResourceRaw();
        IResourceFontViewModel AddResourceFont();

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
    }
}
