using System.ComponentModel;

namespace GrandSeal.Editor.Contracts
{
    public interface IEditorSettings : INotifyPropertyChanged
    {
        string TextureToolsFolder { get; set; }
        string ModelTextureParentFolder { get; set; }

        bool ModelTextureAutoCreateFolder { get; set; }

        void Save(string projectFolder);
        void Load(string projectFolder);
        void Reset();
    }
}
