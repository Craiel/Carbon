#include "stdafx.h"
#include "Server.h"
#include "DataManager.h"
#include "Log.h"
#include <iostream>
#include <vector>

int main()
{
	Carbon::Server::Utils::Log *log = new Carbon::Server::Utils::Log("CarbonServer");
	Carbon::Server::CarbonServer *server = new Carbon::Server::CarbonServer(log);
	Carbon::Server::DataManager *data = new Carbon::Server::DataManager();
	
	log->Info("Starting Server");
	server->Startup();

	log->Info("Waiting for connections");
	while(true)
	{
		server->Receive();
	}

	log->Info("Shutting down");
	server->Shutdown();
	data->~DataManager();
	delete data;
	delete server;
	delete log;

	return 0;
}