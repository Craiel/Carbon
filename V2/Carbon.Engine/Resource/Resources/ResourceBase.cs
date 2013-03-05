using System.IO;

using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Logic;

namespace Carbon.Engine.Resource.Resources
{
    public abstract class ResourceBase : ICarbonResource
    {
        public virtual void Dispose()
        {
        }

        public long Save(Stream target)
        {
            long size;
            using (var writer = new CarbonBinaryFormatter(target))
            {
                this.DoSave(writer);
                size = writer.Position;
            }

            return size;
        }

        public void Save(CarbonBinaryFormatter target)
        {
            this.DoSave(target);
        }

        public void Load(Stream source)
        {
            using (var reader = new CarbonBinaryFormatter(source))
            {
                this.DoLoad(reader);
            }
        }

        public void Load(CarbonBinaryFormatter source)
        {
            this.DoLoad(source);
        }

        protected abstract void DoLoad(CarbonBinaryFormatter source);
        protected abstract void DoSave(CarbonBinaryFormatter target);
    }
}
