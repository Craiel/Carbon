using System.Collections.Generic;

using Carbon.Engine.Resource.Content;

namespace Carbed.Contracts
{
    using Carbon.Engine.Contracts.Resource;

    public delegate void ProjectChangedEventHandler();

    public interface ICarbedLogic : ICarbedBase
    {
        event ProjectChangedEventHandler ProjectChanged;
        
        IContentManager ProjectContent { get; }
        IResourceManager ProjectResources { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(string file);
        void SaveProject(string file);

        void Build(string folder);

        IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue);
    }
}
