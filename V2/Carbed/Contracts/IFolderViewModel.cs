using System.Collections.ObjectModel;
using System.Windows.Input;

using Carbon.Engine.Contracts.Resource;

namespace Carbed.Contracts
{
    public interface IFolderViewModel : ICarbedDocument
    {
        bool IsExpanded { get; set; }

        int? Id { get; }

        string FullPath { get; }

        IFolderViewModel Parent { get; set; }

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

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
