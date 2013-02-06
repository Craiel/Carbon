using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    using Carbon.Engine.Resource.Content;

    public interface IResourceManager
    {
        T Load<T>(ref ResourceLink link) where T : ICarbonResource;

        void Store(ref ResourceLink link, ICarbonResource resource);
        void Replace(ref ResourceLink link, ICarbonResource resource);
        void StoreOrReplace(ref ResourceLink link, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
