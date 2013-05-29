#include "stdafx.h"
#include "DataManager.h"
#include "Log.h"
#include "leveldb/db.h"

namespace Carbon {
namespace Server {
	
	DataManager::DataManager()
	{
		leveldb::Options options;
		options.create_if_missing = true;
		leveldb::DB::Open(options, "carbonServer.db", &db);
	}

	DataManager::~DataManager()
	{
		this->db->~DB();
	}

}}