namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonContent
    {
        ulong Id { get; }

        void Save();
    }
}
