namespace GrandSeal.Editor.Contracts
{
    using System.Windows.Input;

    using CarbonCore.Utils.IO;
    
    public interface IEditorSettingsViewModel : IEditorDocument
    {
        CarbonDirectory TextureToolsFolder { get; }

        IFolderViewModel ModelTextureParentFolder { get; }

        bool ModelTextureAutoCreateFolder { get; set; }

        ICommand CommandSelectTextureToolsFolder { get; }
        ICommand CommandSelectModelTextureParentFolder { get; }

        ICommand CommandReset { get; }
    }
}
