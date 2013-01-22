namespace Carbon.Engine.Resource
{
    using System.Collections.Generic;

    using Carbon.Engine.Contracts.Resource;

    public class ContentManager : IContentManager
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentManager()
        {
            // Todo:
            // - Connect to sql lite which will host the content
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Load<T>(long id) where T : ICarbonContent
        {
            return default(T);
        }

        public IList<T> Load<T>() where T : ICarbonContent
        {
            return null;
        }

        public void Save(ICarbonContent content)
        {
        }

        public void Update(ICarbonContent content)
        {
        }

        public void Delete(ICarbonContent content)
        {
        }
    }
}
