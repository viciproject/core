#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2010 Philippe Leybaert
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

        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        {
            return Type.DefaultBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
        }

        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            return Type.DefaultBinder.BindToField(bindingAttr, match, value, culture);
        }

        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            MethodBase matchingMethod = Type.DefaultBinder.SelectMethod(bindingAttr, match, types, modifiers);

            if (matchingMethod != null)
                return matchingMethod;

            foreach (MethodBase method in match)
                if (ParametersMatch(types, method.GetParameters()))
                    return method;

            return null;
        }

        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        {
            return Type.DefaultBinder.SelectProperty(bindingAttr, match, returnType, indexes, modifiers);
        }

        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            if (value.GetType() == type)
                return value;

            MethodInfo conversionMethod = type.GetMethod("op_Implicit", new[] { value.GetType() });

            if (conversionMethod == null)
                return Type.DefaultBinder.ChangeType(value, type, culture);

            return conversionMethod.Invoke(null, new[] {value});
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            Type.DefaultBinder.ReorderArgumentArray(ref args, state);
        }

        private bool ParametersMatch(Type[] inputParameters, ParameterInfo[] expectedParameters)
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
            return to.GetMethod("op_Implicit", new[] {from}) != null;
        }

    }
}
