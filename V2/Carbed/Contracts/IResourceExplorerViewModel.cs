namespace Carbed.Contracts
{
    public interface IResourceExplorerViewModel : ICarbedTool
    {
        IFolderViewModel Root { get; }
    }
}
