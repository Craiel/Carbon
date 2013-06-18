﻿using System;

using Core.Engine.Resource;
using Core.Engine.Resource.Content;

namespace Core.Engine.Contracts.Resource
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