#include "stdafx.h"
#include "leveldb\db.h"

namespace Carbon {
namespace Server {
	
	class DataManager
	{
	public:
		DataManager();
		~DataManager();

	private:
		leveldb::DB* db;
	};

}}