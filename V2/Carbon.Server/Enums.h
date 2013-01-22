#pragma once

#include "stdafx.h"

namespace Carbon {
namespace Server {

	template<typename definition, typename innerType = typename definition::values>
	class TypedEnum : public definition
	{
		typedef typename definition::values type;
		innerType value;

	public:
		TypedEnum(type entry) : value(entry) {}
		innerType InnerValue() const { return value; }

		bool operator == (const TypedEnum & s) const { return this->value == s.val; }
		bool operator != (const TypedEnum & s) const { return this->value != s.val; }
		bool operator <  (const TypedEnum & s) const { return this->value <  s.val; }
		bool operator <= (const TypedEnum & s) const { return this->value <= s.val; }
		bool operator >  (const TypedEnum & s) const { return this->value >  s.val; }
		bool operator >= (const TypedEnum & s) const { return this->value >= s.val; }
	};

	struct LogLevelEnum {
		enum values { Info, Error, Warning, Debug };
	};
	typedef TypedEnum<LogLevelEnum> LogLevel;

}}