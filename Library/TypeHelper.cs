#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2011 Philippe Leybaert
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
using System.Collections.Generic;
using System.Reflection;

namespace Vici.Core
{
    public static class AttributeHelper
    {
        public static bool HasAttribute<T>(this MemberInfo type, bool inherit) where T : Attribute
        {
            return type.IsDefined(typeof(T), inherit);
        }

        public static T GetAttribute<T>(this MemberInfo type, bool inherit) where T : Attribute
        {
            T[] attributes = (T[])type.GetCustomAttributes(typeof(T), inherit);

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(this MemberInfo type, bool inherit) where T : Attribute
        {
            return (T[])type.GetCustomAttributes(typeof(T), inherit);
        }
        
    }

    public static class TypeHelper
    {
        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>));
        }

        public static bool CanBeNull(this Type type)
        {
            return !type.IsValueType || type.IsNullable();
        }

        public static Type GetRealType(this Type type)
        {
            if (IsNullable(type))
                return type.GetGenericArguments()[0];

            return type;
        }

        public static object DefaultValue(this Type type)
        {
            if (type.CanBeNull())
                return null;

            return Activator.CreateInstance(type);
        }

        public static bool HasAttribute<T>(this Type type, bool inherit) where T:Attribute
        {
            return type.IsDefined(typeof (T), inherit);
        }

        public static T GetAttribute<T>(this Type type, bool inherit) where T:Attribute
        {
            T[] attributes = (T[]) type.GetCustomAttributes(typeof (T), inherit);

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(this Type type, bool inherit) where T : Attribute
        {
            return (T[])type.GetCustomAttributes(typeof(T), inherit);
        }

        #region Method description
        /// <summary>
        /// Finds all types derived from the given type, limiting the search to the given assembly
        /// </summary>
        /// <param name="baseType">The base type or interface to use for finding types</param>
        /// <param name="assembly">The assembly to look into</param>
        /// <returns>An array of all types found in the given assembly which are either derived from the given type, or implement the given interface</returns>
        #endregion
        public static Type[] FindCompatibleTypes(this Assembly assembly, Type baseType)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type != baseType && baseType.IsAssignableFrom(type))
                    types.Add(type);
            }
            
            return types.ToArray();
        }

        #region Method description
        /// <summary>
        /// Finds all types derived from the given type, limiting the search to the given assembly
        /// </summary>
        /// <param name="assembly">The assembly to look into</param>
        /// <returns>An array of all types found in the given assembly which are either derived from the given type, or implement the given interface</returns>
        #endregion
        public static Type[] FindCompatibleTypes<T>(this Assembly assembly)
        {
            return FindCompatibleTypes(assembly, typeof(T));
        }

    }
}