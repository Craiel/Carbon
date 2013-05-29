using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    public interface IEditorSettingsViewModel : IEditorDocument
    {
        string TextureToolsFolder { get; }

        IFolderViewModel ModelTextureParentFolder { get; }

        bool ModelTextureAutoCreateFolder { get; set; }

        ICommand CommandSelectTextureToolsFolder { get; }
        ICommand CommandSelectModelTextureParentFolder { get; }

        ICommand CommandReset { get; }
    }
}
