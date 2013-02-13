using System.Collections.Generic;

using Carbon.Editor.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;
using Carbon.Project.Resource;

namespace Carbed.Contracts
{
    using Carbon.Engine.Contracts.Resource;

    public delegate void ProjectChangedEventHandler(SourceProject project);

    public interface ICarbedLogic : ICarbedBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        SourceProject Project { get; }

        IContentManager ProjectContent { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(string file);
        void SaveProject(string file);

        void Build(string folder);

        object NewResource(EngineResourceType type);
        object NewResource(ProjectResourceType type);

        IList<MetaDataEntry> GetEntryMetaData(object primaryKeyValue);
    }
}
