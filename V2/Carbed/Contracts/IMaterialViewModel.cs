using System.Windows.Input;
using System.Windows.Media;

using Carbon.Engine.Contracts.Resource;

namespace Carbed.Contracts
{
    public interface IMaterialViewModel : ICarbedDocument
    {
        ImageSource PreviewDiffuseImage { get; }
        ImageSource PreviewNormalImage { get; }
        ImageSource PreviewSpecularImage { get; }
        ImageSource PreviewAlphaImage { get; }

        ICommand CommandUpdatePreview { get; }

        void Save(IContentManager target);
    }
}
