using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface INewDialogViewModel : ICarbedBase
    {
        string Name { get; set; }

        ReadOnlyCollection<IDocumentTemplate> Templates { get; }
        IDocumentTemplate SelectedTemplate { get; set; }

        ReadOnlyCollection<IDocumentTemplateCategory> Categories { get; }
        IDocumentTemplateCategory SelectedCategory { get; set; }
    }
}
