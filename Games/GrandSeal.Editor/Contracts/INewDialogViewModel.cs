namespace GrandSeal.Editor.Contracts
{
    using System.Collections.ObjectModel;

    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface INewDialogViewModel : IBaseViewModel
    {
        string Name { get; set; }

        ReadOnlyCollection<IDocumentTemplate> Templates { get; }
        IDocumentTemplate SelectedTemplate { get; set; }

        ReadOnlyCollection<IDocumentTemplateCategory> Categories { get; }
        IDocumentTemplateCategory SelectedCategory { get; set; }
    }
}
