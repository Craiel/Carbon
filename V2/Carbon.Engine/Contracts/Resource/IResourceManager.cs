using System;

using Carbon.Engine.Resource;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IResourceManager : IDisposable
    {
        T Load<T>(string hash) where T : ICarbonResource;

        void Store(string hash, ICarbonResource resource);
        void Replace(string hash, ICarbonResource resource);
        void StoreOrReplace(string hash, ICarbonResource resource);

        void Clear();
        void AddContent(ResourceContent content);
    }
}
