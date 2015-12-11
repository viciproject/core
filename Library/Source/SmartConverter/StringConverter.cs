#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2012 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Vici.Core
{
    public static class StringConverter
    {
        private class TypedStringConverter<T> : IStringConverter
        {
            private readonly IStringConverter<T> _converter;

            public TypedStringConverter(IStringConverter<T> converter)
            {
                _converter = converter;
            }

            public bool TryConvert(string s, Type targetType, out object value)
            {
                value = null;

                if (!typeof(T).Inspector().IsAssignableFrom(targetType))
                    return false;

                T typedValue;

                if (_converter.TryConvert(s, out typedValue))
                {
                    value = typedValue;
                    return true;
                }

                return false;
                
            }
        }

        private static List<IStringConverter> _stringConverters;

        private static readonly object _staticLock = new object();
        private static string[] _dateFormats = new[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy.MM.dd", "yyyy/MM/dd", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" };

        public static void UnregisterAllStringConverters()
        {
            lock (_staticLock)
            {
                _stringConverters = null;
            }
            
        }

        public static void UnregisterStringConverter(IStringConverter stringConverter)
        {
            lock (_staticLock)
            {
                if (_stringConverters != null)
                    _stringConverters.Remove(stringConverter);
            }
        }

        public static void RegisterStringConverter(IStringConverter stringConverter)
        {
            lock (_staticLock)
            {
                _stringConverters = _stringConverters ?? new List<IStringConverter>();

                _stringConverters.Add(stringConverter);
            }
        }

        public static void RegisterStringConverter<T>(IStringConverter<T> stringConverter)
        {
            RegisterStringConverter(new TypedStringConverter<T>(stringConverter));
        }

        public static void RegisterDateFormats(params string[] dateFormats)
        {
            _dateFormats = dateFormats;
        }

        public static void RegisterDateFormat(string dateFormat)
        {
            RegisterDateFormat(dateFormat,false);
            
        }

        public static void RegisterDateFormat(string dateFormat, bool replace)
        {
            if (replace)
            {
                _dateFormats = new[] {dateFormat};
            }
            else
            {
                _dateFormats = _dateFormats.Union(new[] {dateFormat}).ToArray();
            }
        }

        public static T To<T>(this string stringValue, params string[] dateFormats)
        {
            return Convert<T>(stringValue, dateFormats);
        }

        public static object To(this string stringValue, Type targetType, params string[] dateFormats)
        {
            return Convert(stringValue, targetType, dateFormats);
        }

        public static T Convert<T>(this string stringValue, params string[] dateFormats)
        {
            return (T) Convert(stringValue, typeof (T), dateFormats);
        }

        public static object Convert(this string stringValue, Type targetType, params string[] dateFormats)
        {
            if (dateFormats.Length == 0)
                dateFormats = null;

            if (stringValue == null)
                return targetType.Inspector().DefaultValue();

            if (targetType == typeof (string))
                return stringValue;

            object returnValue = null;

            Type type = targetType.Inspector().RealType;

            if (stringValue.Trim().Length == 0)
                return targetType.Inspector().DefaultValue();

            if (_stringConverters != null)
                if (_stringConverters.Any(converter => converter.TryConvert(stringValue, type, out returnValue)))
                    return returnValue;

            if (type == typeof (double) || type == typeof (float))
            {
                double doubleValue;

                if (!Double.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out doubleValue))
                    returnValue = null;
                else
                    returnValue = doubleValue;
            }
            else if (type == typeof (decimal))
            {
                decimal decimalValue;

                if (!Decimal.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out decimalValue))
                    returnValue = null;
                else
                    returnValue = decimalValue;
            }
            else if (type == typeof (Int32) || type == typeof (Int16) || type == typeof (Int64) || type == typeof (SByte))
            {
                long longValue;

                if (!Int64.TryParse(stringValue, out longValue))
                    returnValue = null;
                else
                    returnValue = longValue;
            }
            else if (type == typeof (UInt32) || type == typeof (UInt16) || type == typeof (UInt64) || type == typeof (Byte))
            {
                ulong longValue;

                if (!UInt64.TryParse(stringValue, out longValue))
                    returnValue = null;
                else
                    returnValue = longValue;
            }
            else if (type == typeof (DateTime))
            {
                DateTime dateTime;

                if (!DateTime.TryParseExact(stringValue, dateFormats ?? _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out dateTime))
                {
                    if (!DateTime.TryParse(stringValue, out dateTime))
                    {
                        double? seconds = Convert<double?>(stringValue);

                        if (seconds == null)
                            returnValue = null;
                        else
                            returnValue = new DateTime(1970, 1, 1).AddSeconds(seconds.Value);
                    }
                }
                else
                    returnValue = dateTime;
            }
            else if (type == typeof (bool))
            {
                returnValue = (stringValue == "1" || stringValue.ToUpper() == "Y" || stringValue.ToUpper() == "YES" || stringValue.ToUpper() == "T" || stringValue.ToUpper() == "TRUE");
            }
            else if (type == typeof(char))
            {
                if (stringValue.Length == 1)
                    returnValue = stringValue[0];
                else
                    returnValue = null;
            }
            else if (type.Inspector().IsEnum)
            {
                if (char.IsNumber(stringValue,0))
                {
                    long longValue;

                    if (Int64.TryParse(stringValue, out longValue))
                    {
                        returnValue = Enum.ToObject(type, longValue);

                        if (Enum.IsDefined(type, returnValue))
                            return returnValue;
                    }
                }
                else
                {
                    if (Enum.IsDefined(type, stringValue))
                        return Enum.Parse(type, stringValue, true);
                }

                return targetType.Inspector().DefaultValue();
            }

            if (returnValue == null)
                return targetType.Inspector().DefaultValue();

            try
            {
                return System.Convert.ChangeType(returnValue, type, null);
            }
            catch
            {
                return targetType.Inspector().DefaultValue();
            }
        }
    }
}