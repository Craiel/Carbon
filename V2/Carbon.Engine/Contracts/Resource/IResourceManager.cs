using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IResourceManager
    {
        T Load<T>(string key) where T : ICarbonResource;

        void Store(string key, ICarbonResource resource);
        void Replace(string key, ICarbonResource resource);
        void StoreOrReplace(string key, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
