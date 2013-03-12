using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface ICarbedSettingsViewModel : ICarbedDocument
    {
        string TextureToolsFolder { get; }

        IFolderViewModel ModelTextureParentFolder { get; }

        bool ModelTextureAutoCreateFolder { get; set; }

        ICommand CommandSelectTextureToolsFolder { get; }
        ICommand CommandSelectModelTextureParentFolder { get; }

        ICommand CommandReset { get; }
    }
}
