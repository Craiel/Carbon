namespace Core.Engine.Resource.Resources
{
    using System.IO;

    using Core.Engine.Contracts.Resource;

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
