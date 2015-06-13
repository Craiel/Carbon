namespace GrandSeal.Editor.Contracts
{
    using System.ComponentModel;

    using CarbonCore.Utils.Compat.IO;
    
    public interface IEditorSettings : INotifyPropertyChanged
    {
        CarbonDirectory TextureToolsFolder { get; set; }

        string ModelTextureParentFolderHash { get; set; }

        bool ModelTextureAutoCreateFolder { get; set; }

        void Save(CarbonDirectory projectFolder);
        void Load(CarbonDirectory projectFolder);
        void Reset();
    }
}
