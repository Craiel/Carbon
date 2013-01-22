#include "stdafx.h"

static const int DEFAULT_PORT = 9991;

namespace Carbon {
namespace Server {

	class CarbonServer
	{
	public:
		CarbonServer();

		int Port;

		void Startup();
		void Shutdown();

		void Disconnect(unsigned int client);
		void DisconnectAll();

		void Receive();

	protected:
		SOCKET ListenSocket;

	private:
		std::map<unsigned int, SOCKET> *sessions;

		void InitializeSocket();
	};

}}