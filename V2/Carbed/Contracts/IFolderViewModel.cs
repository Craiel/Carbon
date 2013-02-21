using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IFolderViewModel : ICarbedDocument
    {
        bool IsExpanded { get; set; }

        string FullPath { get; }

        ReadOnlyObservableCollection<ICarbedDocument> Content { get; }

        ICommand CommandAddFolder { get; }
        ICommand CommandDeleteFolder { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandExpandAll { get; }
        ICommand CommandCollapseAll { get; }

        void AddContent(IResourceViewModel content);
        void DeleteContent(IResourceViewModel content);
        void SetExpand(bool expanded);

        void DeleteFolder(IFolderViewModel folder);
    }
}
