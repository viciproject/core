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

using System.Linq;
using System.Collections.Generic;
using System;
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
}