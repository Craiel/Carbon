using System;

using Core.Engine.Resource;

namespace Core.Engine.Contracts.Resource
{
    public interface IResourceManager : IDisposable
    {
        T Load<T>(string hash) where T : ICarbonResource;

        void Store(string hash, ICarbonResource resource);
        void Replace(string hash, ICarbonResource resource);
        void StoreOrReplace(string hash, ICarbonResource resource);
        void Delete(string hash);

        ResourceInfo GetInfo(string hash);

        void ClearCache();
        void AddContent(ResourceContent content);
    }
}
