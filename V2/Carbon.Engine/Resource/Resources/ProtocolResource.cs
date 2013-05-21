using System.IO;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Resources
{
    public abstract class ProtocolResource : ICarbonResource
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Dispose()
        {
        }

        public abstract void Load(Stream source);

        public abstract long Save(Stream target);
    }
}
