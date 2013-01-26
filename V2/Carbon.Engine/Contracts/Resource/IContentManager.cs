using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IContentManager
    {
        ContentQueryResult<T> Load<T>(ContentQuery<T> criteria) where T : ICarbonContent;

        void Save(ICarbonContent content);
    }
}
