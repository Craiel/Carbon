using System;
using System.IO;

namespace Core.Engine.Contracts.Network
{
    using Core.Protocol.Network;

    public interface INetworkMessage
    {
        Guid Id { get; }

        Header.Types.MessageType Type { get; }

        void Send(Stream target);
    }
}
