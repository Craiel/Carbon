using System;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Content
{
    public abstract class ContentEntry : ICarbonContent
    {
        public virtual ICarbonContent Clone(bool fullCopy = false)
        {
            throw new NotImplementedException("Clone is not implemented for " + this.GetType());
        }

        public virtual void LoadFrom(ICarbonContent source)
        {
            throw new NotImplementedException("LoadFrom is not implemented for " + this.GetType());
        }
    }
}
