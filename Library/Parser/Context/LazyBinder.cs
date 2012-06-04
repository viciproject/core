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

namespace Vici.Core.Parser
{
    public class LazyBinder : Binder
    {
        private static readonly LazyBinder _default = new LazyBinder();

        public static LazyBinder Default
        {
            get { return _default; }
        }

        private Binder DefaultTypeBinder
        {
#if NETFX_CORE
            get { return new Binder(); }
#else
            get { return Type.DefaultBinder; }
#endif
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

#endif

        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            MethodBase matchingMethod = DefaultTypeBinder.SelectMethod(bindingAttr, match, types, modifiers);

            if (matchingMethod != null)
                return matchingMethod;

            return match.FirstOrDefault(method => ParametersMatch(types, method.GetParameters()));
        }


        public static bool ParametersMatch(Type[] inputParameters, ParameterInfo[] expectedParameters)
        {
            if (inputParameters.Length != expectedParameters.Length)
                return false;

            for (int i = 0; i < inputParameters.Length; i++)
                if (!CanConvert(inputParameters[i], expectedParameters[i].ParameterType))
                    return false;

            return true;
        }

        private static bool CanConvert(Type from, Type to)
        {
            return to.Inspector().GetMethod("op_Implicit", new[] {from}) != null;
        }

    }
}
