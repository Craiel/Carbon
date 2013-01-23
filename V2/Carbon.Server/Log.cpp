#include "stdafx.h"
#include "Log.h"
#include "Enums.h"

#include <boost\log\core.hpp>
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

		formatted << level.InnerValue() << "\t";
		formatted << message;
		if(exception != NULL)
		{
			formatted << "\n\t" << exception;
		}
		formatted << "\n";
		
		BOOST_LOG_TRIVIAL(debug) << message.c_str();
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