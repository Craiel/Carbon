using System.ComponentModel;

namespace GrandSeal.Editor.Contracts
{
    using Core.Utils.IO;

    public interface IEditorSettings : INotifyPropertyChanged
    {
        string TextureToolsFolder { get; set; }
        string ModelTextureParentFolder { get; set; }

        bool ModelTextureAutoCreateFolder { get; set; }

        void Save(CarbonDirectory projectFolder);
        void Load(CarbonDirectory projectFolder);
        void Reset();
    }
}
