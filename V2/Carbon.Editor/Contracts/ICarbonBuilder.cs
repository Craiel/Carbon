using System.IO;

using Carbon.Editor.Resource;

namespace Carbon.Editor.Contracts
{
    public delegate void CarbonBuilderProgressChangedDelegate(string message, int current, int max);

    public interface ICarbonBuilder
    {
        event CarbonBuilderProgressChangedDelegate ProgressChanged;

        void Build(string target, SourceProject project);

        void Process(Stream target, SourceFolderContent content);
    }
}
