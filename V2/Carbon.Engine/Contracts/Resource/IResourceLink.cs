namespace Carbon.Engine.Contracts.Resource
{
    public interface IResourceLink
    {
        byte[] Hash { get; set; }

        string SourcePath { get; set; }
    }
}
