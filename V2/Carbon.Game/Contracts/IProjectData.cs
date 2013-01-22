using System.IO;

namespace Carbon.Project.Contracts
{
    public interface IProjectData
    {
        void Save(Stream target);
    }
}
