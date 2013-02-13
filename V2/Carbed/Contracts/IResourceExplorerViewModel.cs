namespace Carbed.Contracts
{
    public interface IResourceExplorerViewModel : ICarbedTool
    {
        IProjectFolderViewModel Root { get; }
    }
}
