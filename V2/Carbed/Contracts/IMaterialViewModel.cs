using System.Windows.Input;
using System.Windows.Media;

namespace Carbed.Contracts
{
    public interface IMaterialViewModel : ICarbedDocument
    {
        ImageSource PreviewDiffuseImage { get; }
        ImageSource PreviewNormalImage { get; }
        ImageSource PreviewSpecularImage { get; }
        ImageSource PreviewTransparencyImage { get; }

        ICommand CommandUpdatePreview { get; }
    }
}
