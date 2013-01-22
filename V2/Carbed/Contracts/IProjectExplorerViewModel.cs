namespace Carbed.Contracts
{
    public interface IProjectExplorerViewModel : ICarbedTool
    {
        IProjectFolderViewModel Root { get; }
    }
}
