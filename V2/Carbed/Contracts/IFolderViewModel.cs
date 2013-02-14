using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IFolderViewModel
    {
        string Name { get; set; }

        bool HasName { get; }
        bool IsExpanded { get; set; }

        ReadOnlyObservableCollection<IResourceViewModel> Content { get; }

        ICommand CommandAddFolder { get; }
        ICommand CommandDeleteFolder { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandExpandAll { get; }
        ICommand CommandCollapseAll { get; }

        void AddContent(IResourceViewModel content);
        void DeleteContent(IResourceViewModel content);
        void SetExpand(bool expanded);
    }
}
