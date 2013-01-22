using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IProjectFolderViewModel : IProjectFolderContent
    {
        string Name { get; set; }

        bool HasName { get; }
        bool IsExpanded { get; set; }
        
        ReadOnlyObservableCollection<IProjectFolderContent> Content { get; }

        ICommand CommandAddFolder { get; }
        ICommand CommandDeleteFolder { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandExpandAll { get; }
        ICommand CommandCollapseAll { get; }

        void AddContent(IProjectFolderContent content);
        void DeleteContent(IProjectFolderContent content);
        void SetExpand(bool expanded);
    }
}
