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

namespace Vici.Core
{
#if NETFX_CORE
    public abstract class Binder { }

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

    public class MissingMethodException : Exception
    {
        public MissingMethodException(string name)
        {

        }
    }

#endif

    public class LazyBinder : Binder
    {
        private static readonly LazyBinder _default = new LazyBinder();

        public static LazyBinder Default
        {
            get { return _default; }
        }

#if !NETFX_CORE
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        {
            return DefaultTypeBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
        }

        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            return DefaultTypeBinder.BindToField(bindingAttr, match, value, culture);
        }

        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        {
            return DefaultTypeBinder.SelectProperty(bindingAttr, match, returnType, indexes, modifiers);
        }

        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            if (value.GetType() == type)
                return value;

            MethodInfo conversionMethod = type.Inspector().GetMethod("op_Implicit", new[] { value.GetType() });

            if (conversionMethod == null)
                return DefaultTypeBinder.ChangeType(value, type, culture);

            return conversionMethod.Invoke(null, new[] {value});
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            DefaultTypeBinder.ReorderArgumentArray(ref args, state);
        }


        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] methods, Type[] types, ParameterModifier[] modifiers)
        {
            MethodBase matchingMethod = DefaultTypeBinder.SelectMethod(bindingAttr, methods, types, modifiers);

            if (matchingMethod != null)
                return matchingMethod;

            return methods.FirstOrDefault(method => ParametersMatch(types, method.GetParameters()));
        }
#endif

        private static bool MatchBindingFlags(MethodBase methodBase, BindingFlags flags)
        {
            if (flags == BindingFlags.Default)
                return true;

            if ((flags & BindingFlags.Static) == 0 && methodBase.IsStatic)
                return false;

            if ((flags & BindingFlags.Instance) == 0 && !methodBase.IsStatic)
                return false;

            if ((flags & BindingFlags.Public) != 0 && !methodBase.IsPublic)
                return false;

            if ((flags & BindingFlags.NonPublic) != 0 && !methodBase.IsPrivate)
                return false;

            return true;
        }

        private enum ParameterCompareType
        {
            Exact,
            Assignable,
            Implicit
        }

        private class ParameterComparer : IEqualityComparer<Type>
        {
            private readonly ParameterCompareType _compareType;

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


        public static bool MatchParameters(Type[] parameterTypes, ParameterInfo[] parameters)
        {
            var compareTypes = new[] { ParameterCompareType.Exact, ParameterCompareType.Assignable, ParameterCompareType.Implicit };

            return compareTypes.Any(compareType => MatchParameters(parameterTypes, parameters, compareType));
        }

        private static bool MatchParameters(Type[] parameterTypes, ParameterInfo[] parameters, ParameterCompareType compareType)
        {
            if (parameterTypes.Length != parameters.Length)
                return false;

            if (parameterTypes.Length == 0)
                return true;

            return parameterTypes.SequenceEqual(parameters.Select(p => p.ParameterType), new ParameterComparer(compareType));
        }

        internal static T SelectBestMethod<T>(IEnumerable<T> methods, Type[] parameterTypes, BindingFlags bindingFlags = BindingFlags.Default) where T:MethodBase
        {
            var compareTypes = new[] { ParameterCompareType.Exact, ParameterCompareType.Assignable, ParameterCompareType.Implicit };

            return compareTypes
                .Select(compareType => methods.FirstOrDefault(m => MatchParameters(parameterTypes, m.GetParameters(), compareType) && MatchBindingFlags(m, bindingFlags) ))
                .FirstOrDefault(match => match != null);
        }

        private static object[] ConvertParameters(object[] parameters, ParameterInfo[] parameterTypes)
        {
            var newParameters = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                newParameters[i] = parameters[i].Convert(parameterTypes[i].ParameterType);
            }

            return newParameters;
        }

        public static object Invoke(MethodBase method, object[] parameters)
        {
            object[] p = ConvertParameters(parameters, method.GetParameters());

            if (method is ConstructorInfo)
            {
                return ((ConstructorInfo) method).Invoke(p);
            }
            else
            {
                return method.Invoke(null, p);
            }
        }


        public static object Invoke(MethodBase method, object target, object[] parameters)
        {
            return method.Invoke(target, ConvertParameters(parameters, method.GetParameters()));
        }

    }


}
