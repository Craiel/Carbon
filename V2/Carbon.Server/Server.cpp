#include "stdafx.h"
#include "Server.h"
#include "network.pb.h"

namespace Carbon {
namespace Server {

    CarbonServer::CarbonServer()
    {
        Port = DEFAULT_PORT;

        ListenSocket = INVALID_SOCKET;

        sessions = new std::map<unsigned int, SOCKET>();
    }

    // -------------------------------------------------------------------
    // Private
    // -------------------------------------------------------------------
    void CarbonServer::InitializeSocket()
    {
        WSADATA data;
        struct addrinfo *result = NULL;
        struct addrinfo hints;
        int operationResult;

        operationResult = WSAStartup(MAKEWORD(2, 2), &data);
        if(operationResult != 0)
        {
            throw;
        }

        ZeroMemory(&hints, sizeof(hints));
        hints.ai_family = AF_INET;
        hints.ai_socktype = SOCK_STREAM;
        hints.ai_protocol = IPPROTO_TCP;
        hints.ai_flags = AI_PASSIVE;

        PCSTR port = std::to_string(Port).c_str();
        operationResult = getaddrinfo(NULL, port, &hints, &result);
        if(operationResult != 0)
        {
            throw;
        }

        ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
        if(ListenSocket == INVALID_SOCKET)
        {
            freeaddrinfo(result);
            WSACleanup();
            throw;
        }

        u_long mode = 1; // Non-blocking
        operationResult = ioctlsocket(ListenSocket, FIONBIO, &mode);
        if(operationResult == SOCKET_ERROR)
        {
            closesocket(ListenSocket);
            WSACleanup();
            throw;
        }

        operationResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
        if(operationResult == SOCKET_ERROR)
        {
            freeaddrinfo(result);
            closesocket(ListenSocket);
            WSACleanup();
            throw;
        }

        freeaddrinfo(result);

        operationResult = listen(ListenSocket, SOMAXCONN);
        if(operationResult == SOCKET_ERROR)
        {
            closesocket(ListenSocket);
            WSACleanup();
            throw;
        }
    }

    // -------------------------------------------------------------------
    // Public
    // -------------------------------------------------------------------
    void CarbonServer::Startup()
    {
        Shutdown();
        InitializeSocket();
    }

    void CarbonServer::Shutdown()
    {
        if(ListenSocket == INVALID_SOCKET)
        {
            return;
        }

        // Todo: Broadcast message to clients that we are shutting down
        DisconnectAll();

        closesocket(ListenSocket);
        WSACleanup();

        ListenSocket = INVALID_SOCKET;
    }

    void CarbonServer::Disconnect(unsigned int client)
    {
    }

    void CarbonServer::DisconnectAll()
    {
    }

    void CarbonServer::Receive()
    {
    }
}}
    