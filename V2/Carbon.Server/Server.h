#include "stdafx.h"
#include "Log.h"

static const int DEFAULT_PORT = 9991;

namespace Carbon {
namespace Server {

	class CarbonServer
	{
	public:
		CarbonServer(Carbon::Server::Utils::Log *log);

		int Port;

		void Startup();
		void Shutdown();

		void Disconnect(unsigned int client);
		void DisconnectAll();

		void Receive();

	protected:
		SOCKET ListenSocket;

	private:
		Carbon::Server::Utils::Log *log;

		std::map<unsigned int, SOCKET> *sessions;

		void InitializeSocket();
	};

}}