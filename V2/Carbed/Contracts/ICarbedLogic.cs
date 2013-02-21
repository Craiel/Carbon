using System.Collections.Generic;

using Carbon.Engine.Resource.Content;

namespace Carbed.Contracts
{
    public delegate void ProjectChangedEventHandler();

    public interface ICarbedLogic : ICarbedBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        bool HasProjectLoaded { get; }

        IReadOnlyCollection<IMaterialViewModel> Materials { get; }
        IReadOnlyCollection<IFolderViewModel> Folders { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(string file);
        void SaveProject(string file);

        void Reload();

        IMaterialViewModel AddMaterial();
        void Save(IMaterialViewModel material);
        void Delete(IMaterialViewModel material);
        IMaterialViewModel Clone(IMaterialViewModel source);

        IFolderViewModel AddFolder { get; }
        void Save(IFolderViewModel folder);
        void Delete(IFolderViewModel folder);
        IFolderViewModel Clone(IFolderViewModel source);

        IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue);
        IList<ResourceTree> GetResourceTreeChildren(int parent);
    }
}
