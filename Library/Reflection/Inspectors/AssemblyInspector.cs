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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vici.Core
{
    public class AssemblyInspector
    {
        private Assembly _assembly;

        public AssemblyInspector(Assembly assembly)
        {
            _assembly = assembly;
        }

        #region Method description
        /// <summary>
        /// Finds all types derived from the given type, limiting the search to the given assembly
        /// </summary>
        /// <param name="baseType">The base type or interface to use for finding types</param>
        /// <param name="assembly">The assembly to look into</param>
        /// <returns>An array of all types found in the given assembly which are either derived from the given type, or implement the given interface</returns>
        #endregion
        public Type[] FindCompatibleTypes(Type baseType)
        {
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();

            return _assembly.DefinedTypes.Where(type => type != baseTypeInfo && baseTypeInfo.IsAssignableFrom(type)).ToArray().ConvertAll(typeInfo => typeInfo.AsType());
        }

        #region Method description
        /// <summary>
        /// Finds all types derived from the given type, limiting the search to the given assembly
        /// </summary>
        /// <param name="assembly">The assembly to look into</param>
        /// <returns>An array of all types found in the given assembly which are either derived from the given type, or implement the given interface</returns>
        #endregion
        public Type[] FindCompatibleTypes<T>()
        {
            return FindCompatibleTypes(typeof(T));
        }

    }
}