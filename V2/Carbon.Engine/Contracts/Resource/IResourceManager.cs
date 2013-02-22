using System;

using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    using Carbon.Engine.Resource.Content;

    public interface IResourceManager : IDisposable
    {
        ResourceLink GetLink(string path);

        T Load<T>(ResourceLink link) where T : ICarbonResource;

        void Store(ResourceLink link, ICarbonResource resource);
        void Replace(ResourceLink link, ICarbonResource resource);
        void StoreOrReplace(ResourceLink link, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
