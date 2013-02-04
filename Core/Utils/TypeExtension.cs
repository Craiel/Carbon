﻿using System;
using System.IO;

namespace Core.Utils
{
    public static class TypeExtension
    {
        public static object ConvertValue(this Type type, object source)
        {
            bool isNullable = type.IsClass;
            Type targetType = type;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                targetType = targetType.GetGenericArguments()[0];
            }

            if (source == null || source is DBNull)
            {
                if (isNullable)
                {
                    return default(Type);
                }
                
                throw new InvalidDataException("Target type is not nullable, source value is invalid");
            }

            if (source.GetType() == type)
            {
                return source;
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(source);
            }

            if (targetType == typeof(uint))
            {
                return Convert.ToUInt32(source);
            }

            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(source);
            }

            if (targetType == typeof(float))
            {
                return Convert.ToSingle(source);
            }

            if (targetType == typeof(DateTime))
            {
                return Convert.ToDateTime(source);
            }

            if (targetType.IsEnum)
            {
                string name;
                if (source is string)
                {
                    name = source as string;
                }
                else
                {
                    name = Enum.GetName(targetType, source);
                }

                return Enum.Parse(targetType, name);
            }

            throw new NotImplementedException(string.Format("Can not get Typed value of {0} for target type {1}", source.GetType(), targetType));
        }
    }
}
