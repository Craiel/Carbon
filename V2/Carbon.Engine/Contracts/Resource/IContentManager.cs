using System.Collections.Generic;

namespace Carbon.Engine.Contracts.Resource
{
    public interface IContentManager
    {
        T Load<T>(long id) where T : ICarbonContent;
        IList<T> Load<T>() where T : ICarbonContent; 

        void Save(ICarbonContent content);
        void Update(ICarbonContent content);
        void Delete(ICarbonContent content);
    }
}
