using System;
using System.Reflection;

namespace SastCSharpTest.Helper;

public static class GetEnumHelper
{
    public static TEnum GetEnum<TEnum>(string text)
        where TEnum : struct
    {
        if (!typeof(TEnum).GetTypeInfo().IsEnum)
        {
            throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
        }
        return (TEnum)Enum.Parse(typeof(TEnum), text);
    }
}