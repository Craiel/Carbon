using System;
using System.IO;

using Carbon.Engine.Network;

namespace Carbon.Engine.Contracts.Network
{
    public interface INetworkMessage
    {
        Guid Id { get; }

        Header.Types.MessageType Type { get; }

        void Send(Stream target);
    }
}
