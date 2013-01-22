namespace Carbon.Engine.Contracts.Network
{
    using System;
    using System.Collections.Generic;

    using Carbon.Engine.Contracts.Logic;

    public interface INetworkClient : IEngineComponent
    {
        Guid Id { get; }

        bool IsConnected { get; }
        bool AutoConnect { get; set; }

        void Connect(INetworkTarget target);
        void Disconnect();

        IList<INetworkMessage> GetMessages();
        void Send(INetworkMessage payload);
    }
}
