using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;

namespace Carbed.Contracts
{
    public interface ITextureFontViewModel : ICarbedDocument, IProjectFolderContent
    {
        bool HasName { get; }
        
        int FontSize { get; set; }

        IReadOnlyCollection<Font> SelectableFonts { get; }
        Font SelectedFont { get; set; }

        ImageSource PreviewImage { get; }

        ICommand CommandUpdatePreview { get; }
    }
}
