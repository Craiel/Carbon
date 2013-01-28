using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IResourceManager
    {
        T Load<T>(ResourceLink link) where T : ICarbonResource;

        void Store(ref ResourceLink link, ICarbonResource resource);
        void Replace(ResourceLink link, ICarbonResource resource);
        void StoreOrReplace(ref ResourceLink link, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
