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
using System.Globalization;
using System.Linq;

namespace Vici.Core
{
    public static class ObjectConverter
    {
        public static T Convert<T>(this object value)
        {
            return (T) Convert(value, typeof (T));
        }

        public static object Convert(this object value, Type targetType)
        {
			object defaultReturnValue = targetType.Inspector().DefaultValue();

            if (value == null)
				return defaultReturnValue;

            if (targetType == typeof (object))
                return value;

			targetType = targetType.Inspector().RealType;
			Type sourceType = value.GetType();

			if (sourceType == targetType)
                return value;

			var implicitOperator = targetType.Inspector().GetMethod("op_Implicit", new [] {sourceType});

            if (implicitOperator != null)
                return implicitOperator.Invoke(null, new [] {value});

            if (value is string)
                return StringConverter.Convert((string)value, targetType);

			if (targetType == typeof(string))
            {
                Type valueType = value.GetType();

                if (valueType == typeof(decimal))
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                if (valueType == typeof(double))
                    return ((double)value).ToString(CultureInfo.InvariantCulture);
                if (valueType == typeof(float))
                    return ((float)value).ToString(CultureInfo.InvariantCulture);

                return value.ToString();
            }

			if (targetType == typeof (Guid) && value is byte[])
                return new Guid((byte[]) value);

			if (targetType == typeof (byte[]) && value is Guid)
                return ((Guid) value).ToByteArray();

			if (targetType.Inspector().IsEnum)
            {
                try
                {
                    value = System.Convert.ToInt64(value);

					value = Enum.ToObject(targetType, value);
                }
                catch
                {
					return defaultReturnValue;
                }

                // Enum.IsDefined is the quickest check, but
                // produces false negatives on enums defined with [Flags],
                // i.e. enums with bits that can be combined
                if (Enum.IsDefined(targetType, value))
                    return value;
                else
                {
                    // see http://stackoverflow.com/questions/4950001/enum-isdefined-with-flagged-enums
                    decimal d;
                    if (!decimal.TryParse(value.ToString(), out d))
                        return value;
                }
                return defaultReturnValue;            
            }

			if (targetType.Inspector().IsAssignableFrom(value.GetType()))
                return value;

			if (targetType.IsArray && sourceType.IsArray)
			{
				Type targetArrayType = targetType.GetElementType();
				Array sourceArray = value as Array;

				Array array = Array.CreateInstance(targetArrayType, new [] { sourceArray.Length }, new [] { 0 });

				for (int i = 0; i < sourceArray.Length; i++)
				{
					array.SetValue(sourceArray.GetValue(i).Convert(targetArrayType), i);
				}

				return array;
			}


            try
            {
				return System.Convert.ChangeType(value, targetType, null);
            }
            catch
            {
				return defaultReturnValue;
            }
        }

    }
}