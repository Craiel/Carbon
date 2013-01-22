using System.IO;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class RawResource : ICarbonResource
    {
        public RawResource(Stream source)
        {
            this.Data = new byte[source.Length];
            source.Read(this.Data, 0, this.Data.Length);
        }

        public byte[] Data { get; set; }

        public long Save(Stream target)
        {
            target.Write(this.Data, 0, this.Data.Length);
            return this.Data.Length;
        }
    }
}
