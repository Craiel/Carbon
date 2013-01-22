#include "stdafx.h"
#include "Enums.h"

#define DEFAULT_TIMEFORMAT "%H:%M:%S"

namespace Carbon {
namespace Server {
namespace Utils {
	
	class Log
	{
	public:
		Log(std::string name);
		virtual ~Log();

		std::string TimeFormat;

		void Info(std::string message);
		void Warning(std::string message);
		void Error(std::string message, std::exception *exception = NULL);
		void Debug(std::string message);

	private:
		std::string name;
		std::ostringstream formatted;

		void LogMessage(LogLevel level, std::string message, std::exception *exception = NULL);
		std::string GetTimeString(time_t time);
	};

}}}