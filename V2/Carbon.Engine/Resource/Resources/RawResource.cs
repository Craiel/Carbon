using System.IO;

using Google.ProtocolBuffers;

namespace Carbon.Engine.Resource.Resources
{
    public class RawResource : ProtocolResource
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] Data { get; set; }

        public override void Load(Stream source)
        {
            this.Data = Protocol.Resource.Raw.ParseFrom(source).Data.ToByteArray();
        }

        public override long Save(Stream target)
        {
            var builder = new Protocol.Resource.Raw.Builder();
            builder.SetData(ByteString.CopyFrom(this.Data));
            Protocol.Resource.Raw entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
