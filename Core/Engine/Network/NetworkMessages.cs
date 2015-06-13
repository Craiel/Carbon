namespace Core.Engine.Network
{
    using System;
    using System.IO;

    using CarbonCore.Protocol.Network;

    using Core.Engine.Contracts.Network;

    using Google.ProtocolBuffers;

    public abstract class NetworkMessageBase : INetworkMessage
    {
        protected NetworkMessageBase(Client client)
        {
            this.Id = new Guid(client.Id.ToByteArray());
            this.Version = client.Version;
        }

        protected NetworkMessageBase()
        {
            this.Id = NetworkClient.Guid;
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }

        public abstract Header.Types.MessageType Type { get; }

        public abstract void Send(Stream target);

        public Client GetClientInfo()
        {
            var builder = Client.CreateBuilder();
            builder.Id = ByteString.CopyFrom(this.Id.ToByteArray());
            builder.Version = this.Version;
            return builder.Build();
        }
    }

    public class NetworkConnect : NetworkMessageBase
    {
        public override Header.Types.MessageType Type
        {
            get
            {
                return Header.Types.MessageType.Connect;
            }
        }

        public override void Send(Stream target)
        {
            throw new NotImplementedException();
        }
    }
}
