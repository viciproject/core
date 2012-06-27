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
            if (value == null)
                return targetType.Inspector().DefaultValue();

            Type type = targetType.Inspector().RealType;

            if (value.GetType() == type)
                return value;

            var implicitOperator = type.Inspector().GetMethod("op_Implicit", new [] {value.GetType()});

            if (implicitOperator != null)
                return implicitOperator.Invoke(null, new object[] {value});
            
            if (value is string)
                return StringConverter.Convert((string) value, targetType);

            if (type == typeof(string))
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

            if (type == typeof (Guid) && value is byte[])
                return new Guid((byte[]) value);

            if (type == typeof (byte[]) && value is Guid)
                return ((Guid) value).ToByteArray();

            if (type.Inspector().IsEnum)
            {
                try
                {
                    value = System.Convert.ToInt64(value);

                    value = Enum.ToObject(type, value);
                }
                catch
                {
                    return targetType.Inspector().DefaultValue();
                }

                return Enum.IsDefined(type, value) ? 
                            value 
                            : 
                            targetType.Inspector().DefaultValue();
            }

            if (type.Inspector().IsAssignableFrom(value.GetType()))
                return value;

            try
            {
                return System.Convert.ChangeType(value, type, null);
            }
            catch
            {
                return targetType.Inspector().DefaultValue();
            }
        }

    }
}