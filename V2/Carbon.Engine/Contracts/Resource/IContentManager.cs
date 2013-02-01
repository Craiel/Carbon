using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IContentManager
    {
        ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent;
        ContentQueryResult Load(ContentQuery criteria);

        void Save(ICarbonContent content);
    }
}
