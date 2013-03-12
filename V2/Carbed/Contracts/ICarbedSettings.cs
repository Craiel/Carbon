using System.ComponentModel;

namespace Carbed.Contracts
{
    public interface ICarbedSettings : INotifyPropertyChanged
    {
        string TextureToolsFolder { get; set; }
        string ModelTextureParentFolder { get; set; }

        bool ModelTextureAutoCreateFolder { get; set; }

        void Save(string projectFolder);
        void Load(string projectFolder);
        void Reset();
    }
}
