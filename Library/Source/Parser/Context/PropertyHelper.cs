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
using System.Reflection;

namespace Vici.Core.Parser
{
    public static class PropertyHelper
    {
        public static bool TryGetValue(object obj, string propertyName, out object value, out Type type)
        {
            value = null;
            type = typeof(object);

            if (obj is IDynamicObject)
                return ((IDynamicObject) obj).TryGetValue(propertyName, out value, out type);

            Type targetType = obj.GetType();

            MemberInfo[] members = targetType.Inspector().GetMember(propertyName);

            if (members.Length == 0)
            {
                PropertyInfo indexerPropInfo = targetType.Inspector().GetIndexer(new[] { typeof(string) });

                if (indexerPropInfo != null)
                {
                    value = indexerPropInfo.GetValue(obj, new object[] { propertyName });
                    type = (value != null && indexerPropInfo.PropertyType == typeof(object)) ? value.GetType() : typeof(object);

                    return true;
                }

                return false;
            }

            if (members.Length >= 1 && members[0] is MethodInfo)
            {
                value = new InstanceMethod(targetType, propertyName, obj);
                type = typeof(InstanceMethod);

                return true;
            }

            MemberInfo member = members[0];

            if (members.Length > 1) // CoolStorage, ActiveRecord and Dynamic Proxy frameworks sometimes return > 1 member
            {
                foreach (MemberInfo mi in members)
                    if (mi.DeclaringType == obj.GetType())
                        member = mi;
            }

            if (member is FieldInfo)
            {
                value = ((FieldInfo)member).GetValue(obj);
                type = ((FieldInfo)member).FieldType;

                return true;
            }

            if (member is PropertyInfo)
            {
                value = ((PropertyInfo)member).GetValue(obj, null);
                type = ((PropertyInfo)member).PropertyType;

                return true;
            }

            return false;
        }

        public static bool Exists(object obj, string propertyName)
        {
            object value;
            Type type;

            return TryGetValue(obj, propertyName, out value, out type);
        }

    }
}