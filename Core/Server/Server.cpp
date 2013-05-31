#include "stdafx.h"
#include "Server.h"
#include "Log.h"

namespace Carbon {
namespace Server {

    CarbonServer::CarbonServer(Carbon::Server::Utils::Log *log)
    {
        this->log = log;
        Port = DEFAULT_PORT;

        ListenSocket = INVALID_SOCKET;

        sessions = new std::map<unsigned int, SOCKET>();
    }

    // -------------------------------------------------------------------
    // Private
    // -------------------------------------------------------------------
    void CarbonServer::InitializeSocket()
    {
        this->log->Info("Initializing Socket");

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
            this->log->Error("Failed to get address information");
			return;
        }

        ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
        if(ListenSocket == INVALID_SOCKET)
        {
            freeaddrinfo(result);
            WSACleanup();
            this->log->Error("Socket invalid in creation");
			return;
        }

        u_long mode = 1; // Non-blocking
        operationResult = ioctlsocket(ListenSocket, FIONBIO, &mode);
        if(operationResult == SOCKET_ERROR)
        {
            closesocket(ListenSocket);
            WSACleanup();
            this->log->Error("Socket error in ioctl");
			return;
        }

        operationResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
        if(operationResult == SOCKET_ERROR)
        {
            freeaddrinfo(result);
            closesocket(ListenSocket);
            WSACleanup();
            this->log->Error("Socket error in bind");
			return;
        }

        freeaddrinfo(result);

        operationResult = listen(ListenSocket, SOMAXCONN);
        if(operationResult == SOCKET_ERROR)
        {
            closesocket(ListenSocket);
            WSACleanup();
			this->log->Error(std::string("Socket error in listen").append(std::to_string(this->Port)));
			return;
        }

        this->log->Info(std::string("Socket Initialized on port ").append(std::to_string(this->Port)));
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
		if(this->ListenSocket == INVALID_SOCKET)
		{
			return;
		}

        char buffer[1024] = "";
        int received = recv(this->ListenSocket, buffer, sizeof(buffer), 0);
        if(received < 0)
        {
            int error = WSAGetLastError();

			char *err;
        if (!FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
                           NULL,
                           error,
                           MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // default language
                           (LPTSTR) &err,
                           0,
                           NULL));
						   static char buffer[1024];
        _snprintf_s(buffer, sizeof(buffer), "ERROR: %s: %s\n", "", err);
            return;
			//this->log->Error("Socket Receive returned error: " + error);
        }
    }
}}
    