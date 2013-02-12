using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;

namespace Carbed.Contracts
{
    public interface IFontViewModel : ICarbedDocument
    {
        int? FontSize { get; set; }

        IReadOnlyCollection<Font> SelectableFonts { get; }
        Font SelectedFont { get; set; }

        ImageSource PreviewImage { get; }

        ICommand CommandUpdatePreview { get; }
    }
}
