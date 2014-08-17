using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    using CarbonCore.ToolFramework.Contracts;

    public interface INewDialogViewModel : IBaseViewModel
    {
        string Name { get; set; }

        ReadOnlyCollection<IDocumentTemplate> Templates { get; }
        IDocumentTemplate SelectedTemplate { get; set; }

        ReadOnlyCollection<IDocumentTemplateCategory> Categories { get; }
        IDocumentTemplateCategory SelectedCategory { get; set; }
    }
}
