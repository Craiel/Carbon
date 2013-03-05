using Carbon.Engine.Logic;

namespace Carbon.Engine.Resource.Resources
{
    public class RawResource : ResourceBase
    {
        private byte[] data;
        
        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            source.Read(out this.data);
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Data);
        }
    }
}
