#include "stdafx.h"
#include "Log.h"
#include "Enums.h"

#include <boost\log\trivial.hpp>

namespace Carbon {
namespace Server {
namespace Utils {

	Log::Log(std::string name)
	{
		this->name = name;

		this->TimeFormat = DEFAULT_TIMEFORMAT;
	}

	Log::~Log()
	{

	}

	void Log::LogMessage(LogLevel level, std::string message, std::exception *exception)
	{
		formatted.str(std::string());

		formatted << this->GetTimeString(time(0)) << "\t";
		formatted << level.InnerValue() << "\t";
		formatted << message;
		if(exception != NULL)
		{
			formatted << "\n\t" << exception;
		}
		formatted << "\n";
		
		fprintf(stdout, formatted.str().c_str());

		BOOST_LOG_TRIVIAL(debug) << message.c_str();
	}

	std::string Log::GetTimeString(time_t time)
	{
		struct tm *info = new tm();
		localtime_s(info, &time);

		std::string buffer;		
		int length = 0;
		int size = this->TimeFormat.size();
		do
		{
			buffer.resize(size + 1);
			length = strftime(&buffer[0], buffer.size(), this->TimeFormat.c_str(), info);
			size *= 2;
		} while(length == 0);
		buffer.resize(length);
		return buffer;
	}

	void Log::Info(std::string message)
	{
		this->LogMessage(LogLevel::Info, message);
	}

	void Log::Warning(std::string message)
	{
		this->LogMessage(LogLevel::Warning, message);
	}

	void Log::Error(std::string message, std::exception *exception)
	{
		this->LogMessage(LogLevel::Error, message, exception);
	}

	void Log::Debug(std::string message)
	{
		this->LogMessage(LogLevel::Debug, message);
	}

}}}