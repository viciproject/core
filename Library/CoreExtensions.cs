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
using System.Globalization;
using System.Linq;
using System.Reflection;
using Vici.Core.Parser;

namespace Vici.Core
{
#if NETFX_CORE
    public delegate TOutput Converter<TInput, TOutput>(TInput value);
#endif
    public static class CoreExtensions
    {
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter) 
        {  
            if (array == null)  
                throw new ArgumentException();  
#if WINDOWS_PHONE || SILVERLIGHT || NETFX_CORE
            return (from item in array select converter(item)).ToArray();  
#else
            return Array.ConvertAll(array,converter);
#endif
        } 

#if NETFX_CORE
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
            

        }
#endif
    }

#if NETFX_CORE
    [Flags]
    public enum BindingFlags
    {
        Public = 16,
        NonPublic = 32,
        Static = 8,
        Instance = 4,
        DeclaredOnly = 2,
        Default = 0
    }

    public class Binder
    {
        /*
        public abstract MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state);
        public abstract FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture);
         */
        public virtual MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            return BestMatch(match, types);
            //return match.FirstOrDefault(method => LazyBinder.ParametersMatch(types, method.GetParameters()));
        }
        /*
        public abstract PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers);
        public abstract object ChangeType(object value, Type type, CultureInfo culture);
        public abstract void ReorderArgumentArray(ref object[] args, object state);
        */


        private enum ParameterCompareType
        {
            Exact,
            Assignable,
            Implicit
        }

        private class ParameterComparer : IEqualityComparer<Type>
        {
            private ParameterCompareType _compareType;

            public ParameterComparer(ParameterCompareType compareType)
            {
                _compareType = compareType;
            }

            public bool Equals(Type x, Type y)
            {
                switch (_compareType)
                {
                    case ParameterCompareType.Exact:
                        return x == y;
                    case ParameterCompareType.Assignable:
                        return y.GetTypeInfo().IsAssignableFrom(x.GetTypeInfo());
                    case ParameterCompareType.Implicit:
                        return y.Inspector().GetMethod("op_Implicit", new[] { x }) != null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }


        private bool IsMatch(Type[] parameterTypes, ParameterInfo[] parameters, ParameterCompareType compareType)
        {
            return parameterTypes.SequenceEqual(parameters.Select(p => p.ParameterType), new ParameterComparer(compareType));
        }

        internal MethodBase BestMatch(IEnumerable<MethodBase> methods, Type[] parameterTypes)
        {
            var compareTypes = new[] {ParameterCompareType.Exact, ParameterCompareType.Assignable, ParameterCompareType.Implicit};

            return compareTypes.Select(compareType => methods.FirstOrDefault(m => IsMatch(parameterTypes, m.GetParameters(), compareType))).FirstOrDefault(match => match != null);
        }
    }

    public class ParameterModifier
    {
        
    }

    
#endif
}
