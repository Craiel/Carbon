namespace Core.Engine.Contracts.Resource
{
    using System;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.Utils.Compat.IO;

    using Core.Engine.Resource;

    public interface IResourceManager : IDisposable
    {
        T Load<T>(string hash) where T : ICarbonResource;

        void SetRoot(CarbonDirectory directory);

        void Store(string hash, ICarbonResource resource);
        void Replace(string hash, ICarbonResource resource);
        void StoreOrReplace(string hash, ICarbonResource resource);
        void Delete(string hash);

        ResourceInfo GetInfo(string hash);

        void ClearCache();
        void AddContent(ResourceContent content);
    }
}
