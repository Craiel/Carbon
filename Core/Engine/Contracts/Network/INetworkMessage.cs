namespace Core.Engine.Contracts.Network
{
    using System;
    using System.IO;

    using Core.Protocol.Network;

    public interface INetworkMessage
    {
        Guid Id { get; }

        Header.Types.MessageType Type { get; }

        void Send(Stream target);
    }
}
