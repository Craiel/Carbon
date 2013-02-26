using System.Collections.ObjectModel;
using System.Windows.Input;

using Carbon.Engine.Contracts.Resource;

namespace Carbed.Contracts
{
    public interface IFolderViewModel : ICarbedDocument
    {
        bool IsExpanded { get; set; }

        int? Id { get; }
        int? ContentCount { get; }

        string FullPath { get; }

        IFolderViewModel Parent { get; set; }

        ReadOnlyObservableCollection<ICarbedDocument> Content { get; }

        ICommand CommandAddFolder { get; }
        ICommand CommandDeleteFolder { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandExpandAll { get; }
        ICommand CommandCollapseAll { get; }
        ICommand CommandCopyPath { get; }

        void AddContent(IResourceViewModel content);
        void RemoveContent(IResourceViewModel content);
        void SetExpand(bool expanded);

        void RemoveFolder(IFolderViewModel folder);

        void Save(IContentManager target, IResourceManager resourceTarget);
        void Delete(IContentManager target, IResourceManager resourceTarget);
    }
}
