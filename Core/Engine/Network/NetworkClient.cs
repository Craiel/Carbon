namespace Core.Engine.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    using CarbonCore.Protocol.Network;

    using Core.Engine.Contracts.Network;
    using Core.Engine.Logic;

    using Google.ProtocolBuffers;

    public class NetworkClient : EngineComponent, INetworkClient
    {
        public static readonly Guid Guid;
        public static readonly Client ClientData;
        
        private readonly Queue<INetworkMessage> pending;
        private readonly Stack<INetworkMessage> received;
        
        private readonly Thread thread;

        private TcpClient client;

        private INetworkTarget connectionTarget;

        private bool isActive = true;
        private ClientState state = ClientState.Idle;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static NetworkClient()
        {
            Guid = Guid.NewGuid();

            Client.Builder clientBuilder = Client.CreateBuilder();
            clientBuilder.SetId(ByteString.CopyFrom(Guid.ToByteArray()));
            ClientData = clientBuilder.Build();
        }

        public NetworkClient()
        {
            this.Id = Guid.NewGuid();

            this.pending = new Queue<INetworkMessage>();
            this.received = new Stack<INetworkMessage>();

            this.thread = new Thread(this.ThreadMain);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        private enum ClientState
        {
            Idle,
            Connecting,
            Connected,
            Disconnecting
        }
        
        public Guid Id { get; private set; }

        public bool IsConnected
        {
            get
            {
                return this.state == ClientState.Connected;
            }
        }

        public bool AutoConnect { get; set; }

        public IList<INetworkMessage> GetMessages()
        {
            lock (this.received)
            {
                var list = new List<INetworkMessage>(this.received);
                this.received.Clear();
                return list;
            }
        }

        public void Send(INetworkMessage payload)
        {
            lock (this.pending)
            {
                this.pending.Enqueue(payload);
            }
        }

        public void Connect(INetworkTarget target)
        {
            lock (this.thread)
            {
                if (this.state == ClientState.Connected)
                {
                    throw new InvalidOperationException("Already connected, disconnect first");
                }

                this.connectionTarget = target;
                this.state = ClientState.Connecting;
            }
        }

        public void Disconnect()
        {
            lock (this.thread)
            {
                this.state = ClientState.Disconnecting;
            }
        }

        private void ThreadMain()
        {
            while (this.isActive)
            {
                switch (this.state)
                {
                    case ClientState.Connecting:
                        {
                            if (this.TryConnect())
                            {
                                this.state = ClientState.Connected;
                                this.Send(new NetworkConnect());
                            }

                            break;
                        }

                    case ClientState.Connected:
                        {
                            this.Send();
                            this.Receive();

                            break;
                        }

                    case ClientState.Disconnecting:
                        {
                            if (this.client != null)
                            {
                                this.client.Close();
                                this.client = null;
                            }

                            break;
                        }

                    default:
                        {
                            Thread.Sleep(50);
                            break;
                        }
                }
            }
        }

        private bool TryConnect()
        {
            try
            {
                this.client = new TcpClient(this.connectionTarget.Address, this.connectionTarget.Port);
                if (this.client.Connected)
                {
                    return true;
                }

                this.client.Close();
                this.client = null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Connection attempt failed for {0}:{1}", e, this.connectionTarget.Address, this.connectionTarget.Port);
            }

            return false;
        }

        private void Send()
        {
            if (this.pending.Count > 0)
            {
                lock (this.pending)
                {
                    INetworkMessage message = this.pending.Dequeue();
                    this.DoSend(message);
                }
            }
        }

        private void DoSend(INetworkMessage message)
        {
            try
            {
                using (Stream stream = this.client.GetStream())
                {
                    // Build the header
                    var headerBuilder = Header.CreateBuilder();
                    headerBuilder.Type = message.Type;

                    byte[] header = headerBuilder.Build().ToByteArray();

                    // Send the data out
                    stream.Write(header, 0, header.Length);
                    message.Send(stream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Error while trying to send data", e);
            }
        }

        private void Receive()
        {
            try
            {
                using (Stream stream = this.client.GetStream())
                {
                    var header = Header.ParseFrom(stream);
                    if (header.IsInitialized)
                    {
                        switch (header.Type)
                        {
                            case Header.Types.MessageType.Unknown:
                                {
                                    System.Diagnostics.Trace.TraceWarning("Header received with unknown message type");
                                    break;
                                }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceWarning("Received packet was properly formatted");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Error while trying to receive data", e);
            }
        }
    }
}
