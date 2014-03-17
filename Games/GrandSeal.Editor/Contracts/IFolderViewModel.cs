namespace GrandSeal.Editor.Contracts
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts.Resource;

    public interface IFolderViewModel : IEditorDocument
    {
        bool IsExpanded { get; set; }

        int? Id { get; }
        int? ContentCount { get; }

        string Hash { get; }
        CarbonDirectory FullPath { get; }

        IFolderViewModel Parent { get; set; }

        ReadOnlyObservableCollection<IEditorDocument> Content { get; }

        ICommand CommandAddExistingResources { get; }
        ICommand CommandAddFolder { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandExpandAll { get; }
        ICommand CommandCollapseAll { get; }
        ICommand CommandCopyPath { get; }

        void AddContent(IResourceViewModel content);
        void RemoveContent(IResourceViewModel content);
        void SetExpand(bool expanded);

        IFolderViewModel AddFolder();
        void RemoveFolder(IFolderViewModel folder);

        void Save(IContentManager target, IResourceManager resourceTarget, bool force);
        void Delete(IContentManager target, IResourceManager resourceTarget);
    }
}
