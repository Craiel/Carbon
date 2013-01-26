using System.Collections.ObjectModel;

namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonContent
    {
        ulong Id { get; }

        ReadOnlyCollection<IResourceLink> Resources { get; }

        void Save();
    }
}
