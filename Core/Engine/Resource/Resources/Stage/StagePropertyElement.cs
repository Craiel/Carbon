using System;
using System.Text;

using Google.ProtocolBuffers;

namespace Core.Engine.Resource.Resources.Stage
{
    public abstract class StagePropertyElement
    {
        public string Id { get; set; }
        
        public virtual Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            return new Protocol.Resource.StageProperty.Builder { Id = this.Id };
        }
    }

    public class StagePropertyElementString : StagePropertyElement
    {
        public StagePropertyElementString()
        {
        }

        public StagePropertyElementString(Protocol.Resource.StageProperty source)
            : this()
        {
            this.Value = source.Data.ToStringUtf8();
        }

        public string Value { get; set; }

        public override Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            Protocol.Resource.StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(this.Value, Encoding.UTF8);
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.String;
            return builder;
        }
    }

    public class StagePropertyElementFloat : StagePropertyElement
    {
        public StagePropertyElementFloat()
        {
        }

        public StagePropertyElementFloat(Protocol.Resource.StageProperty source)
            : this()
        {
            this.Value = BitConverter.ToSingle(source.Data.ToByteArray(), 0);
        }

        public float Value { get; set; }

        public override Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            Protocol.Resource.StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(BitConverter.GetBytes(this.Value));
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.Float;
            return builder;
        }
    }

    public class StagePropertyElementInt : StagePropertyElement
    {
        public StagePropertyElementInt()
        {
        }

        public StagePropertyElementInt(Protocol.Resource.StageProperty source)
            : this()
        {
            this.Value = BitConverter.ToInt32(source.Data.ToByteArray(), 0);
        }

        public int Value { get; set; }

        public override Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            Protocol.Resource.StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(BitConverter.GetBytes(this.Value));
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.Int;
            return builder;
        }
    }
}
