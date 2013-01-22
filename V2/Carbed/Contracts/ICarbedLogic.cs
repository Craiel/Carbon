using Carbon.Editor.Resource;
using Carbon.Engine.Resource;
using Carbon.Project.Resource;

namespace Carbed.Contracts
{
    public delegate void ProjectChangedEventHandler(SourceProject project);

    public interface ICarbedLogic : ICarbedBase
    {
        event ProjectChangedEventHandler ProjectChanged;

        SourceProject Project { get; }
        
        void NewProject();
        void CloseProject();
        void OpenProject(string file);
        void SaveProject(string file);

        void Build(string folder);

        object NewResource(EngineResourceType type, string name);
        object NewResource(ProjectResourceType type, string name);
    }
}
