using System;

using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IContentManager : IDisposable
    {
        ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent;
        ContentQueryResult Load(ContentQuery criteria, bool useResultCache = true);

        T Load<T>(ContentLink link) where T : ICarbonContent;

        void Save(ICarbonContent content);
        void Delete(ICarbonContent content);

        ContentLink ResolveLink(int id);

        void ClearCache();
    }
}
