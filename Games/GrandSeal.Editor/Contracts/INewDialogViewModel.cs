using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    public interface INewDialogViewModel : IEditorBase
    {
        string Name { get; set; }

        ReadOnlyCollection<IDocumentTemplate> Templates { get; }
        IDocumentTemplate SelectedTemplate { get; set; }

        ReadOnlyCollection<IDocumentTemplateCategory> Categories { get; }
        IDocumentTemplateCategory SelectedCategory { get; set; }
    }
}
