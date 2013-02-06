using System.IO;

using Carbon.Project.Contracts;

namespace Carbon.Project.Data
{
    public abstract class ProjectData : IProjectData
    {
        public abstract void Save(Stream target);
    }
}
