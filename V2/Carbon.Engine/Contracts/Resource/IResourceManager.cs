using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IResourceManager
    {
        ContentLoadResult<T> Load<T>(ContentLoadCriteria<T> criteria) where T : ICarbonContent;

        T Load<T>(string key) where T : ICarbonResource;

        void Save(ICarbonContent content);

        void Store(string key, ICarbonResource resource);
        void Replace(string key, ICarbonResource resource);
        void StoreOrReplace(string key, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
