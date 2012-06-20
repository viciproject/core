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
    internal abstract class MethodDefinition : IMethodDefinition
    {
        protected readonly Type _type;
        protected readonly string _methodName;
        protected readonly MethodInfo _methodInfo;

        protected MethodDefinition(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        protected MethodDefinition(Type type, string methodName)
        {
            _type = type;
            _methodName = methodName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public MethodInfo GetMethodInfo(Type[] parameterTypes)
        {
            if (_methodInfo != null)
                return _methodInfo;

            Type t = _type;

            while (t != null)
            {
                MethodInfo methodInfo = t.Inspector().GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, parameterTypes);

                if (methodInfo != null)
                    return methodInfo;

                t = t.Inspector().BaseType;
            }

            return null;


            //return _methodInfo ?? _type.GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, binder ?? LazyBinder.Default, parameterTypes, null);
        }

        public abstract object Invoke(Type[] types, object[] parameters, out Type returnType);
    }
}