using System.ComponentModel;

using Carbon.Editor.Resource.Collada;

namespace Carbed.Contracts
{
    public interface ITextureSynchronizer : INotifyPropertyChanged
    {
        int Synchronized { get; }
        int NewTextures { get; }
        int Deleted { get; }
        int Missing { get; }

        void SetTarget(IFolderViewModel folder);
        void SetSource(ColladaInfo info);

        void Refresh();
        void Synchronize();
    }
}
