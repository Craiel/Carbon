using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IContentManager
    {
        ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent;
        ContentQueryResult Load(ContentQuery criteria);

        void Save(ICarbonContent content);

        ContentLink ResolveLink(int id);
        ResourceLink ResolveResourceLink(int id);
    }
}
