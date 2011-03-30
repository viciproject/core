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
using System.Reflection;

namespace Vici.Core.Parser
{
    public class CallExpression : Expression
    {
        private readonly Expression _methodExpression;
        private readonly Expression[] _parameters;

        public CallExpression(TokenPosition position, Expression methodExpression, Expression[] parameters) : base(position)
        {
            _methodExpression = methodExpression;
            _parameters = parameters;
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            object methodObject = _methodExpression.Evaluate(context).Value;

            ValueExpression[] parameters = EvaluateExpressionArray(_parameters, context);
            Type[] parameterTypes = parameters.ConvertAll(expr => expr.Type);
            object[] parameterValues = parameters.ConvertAll(expr => expr.Value);

			if (methodObject is MethodDefinition)
			{
				Type returnType;

                return Exp.Value(TokenPosition, ((MethodDefinition)methodObject).Invoke(parameterTypes, parameterValues, out returnType, new LazyBinder()), returnType);
			}

			if (methodObject is ConstructorInfo[])
			{
				ConstructorInfo[] constructors = (ConstructorInfo[]) methodObject;

                MethodBase method = new LazyBinder().SelectMethod(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, constructors, parameterTypes, null);

				if (method == null)
					throw new ExpressionEvaluationException("No match found for constructor " + constructors[0].Name, this);

				object value = ((ConstructorInfo)method).Invoke(parameterValues);

                return Exp.Value(TokenPosition, value, method.ReflectedType);
			}

			if (methodObject is Delegate[])
			{
				Delegate[] delegates = (Delegate[]) methodObject;
				MethodBase[] methods = delegates.ConvertAll<Delegate, MethodBase>(d => d.Method);

                MethodBase method = new LazyBinder().SelectMethod(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, methods, parameterTypes, null);

				if (method == null)
					throw new ExpressionEvaluationException("No match found for delegate " + _methodExpression, this);

				object value = method.Invoke(delegates[Array.IndexOf(methods, method)].Target, parameterValues);

                return Exp.Value(TokenPosition, value, ((MethodInfo)method).ReturnType);
			}

            if (methodObject is Delegate)
            {
                Delegate method = (Delegate) methodObject;

                object value = method.Method.Invoke(method.Target, parameterValues);

                return new ValueExpression(TokenPosition, value, method.Method.ReturnType);
            }

            throw new ExpressionEvaluationException(_methodExpression + " is not a function", this);
        }

        public override string ToString()
        {
            string[] parameters = _parameters.ConvertAll(expr => expr.ToString());

            return "(" + _methodExpression + "(" + String.Join(",", parameters) + "))";
        }
    }
}
