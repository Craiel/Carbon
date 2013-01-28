using System;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class ContentManager : IContentManager
    {
        public ContentQueryResult<T> Load<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            throw new NotImplementedException();
        }

        public void Save(ICarbonContent content)
        {
            throw new NotImplementedException();
        }
    }
}
