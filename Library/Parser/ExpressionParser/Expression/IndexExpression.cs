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
    public class IndexExpression : Expression
    {
        private readonly Expression _target;
        private readonly Expression[] _parameters;

        public IndexExpression(TokenPosition position, Expression target, Expression[] parameters) : base(position)
        {
            _target = target;
            _parameters = parameters;
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            return Evaluate(context, false, null);
        }

        public ValueExpression Evaluate(IParserContext context, bool assign, object newValue)
        {
            ValueExpression targetValue = _target.Evaluate(context);

            Type targetType = targetValue.Type;
            object targetObject = targetValue.Value;

            ValueExpression[] parameters = EvaluateExpressionArray(_parameters, context);
            Type[] parameterTypes = parameters.ConvertAll(expr => expr.Type);
            object[] parameterValues = parameters.ConvertAll(expr => expr.Value);

            if (targetType.IsArray)
            {
                if (targetType.GetArrayRank() != parameters.Length)
                    throw new Exception("Array has a different rank. Number of arguments is incorrect");

                Type returnType = targetType.GetElementType();

                bool useLong = false;

                foreach (Type t in parameterTypes)
                {
                    if (t == typeof(long) || t == typeof(long?))
                        useLong = true;
                    else if (t != typeof(int) & t != typeof(int?) && t != typeof(short) && t != typeof(short?) && t != typeof(ushort) && t != typeof(ushort?))
                        throw new BadArgumentException(t.GetType().Name + " is not a valid type for array indexers", this);
                }

#if !PCL
                if (useLong)
                {
                    long[] indexes = new long[parameters.Length];

                    for (int i=0;i<parameters.Length;i++)
                        indexes[i] = Convert.ToInt64(parameterValues[i]);

                    if (assign)
                        ((Array)targetObject).SetValue(newValue, indexes);

                    return Exp.Value(TokenPosition, ((Array)targetObject).GetValue(indexes), returnType);
                }
                else
#endif
                {
                    int[] indexes = new int[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                        indexes[i] = Convert.ToInt32(parameterValues[i]);

                    if (assign)
                        ((Array)targetObject).SetValue(newValue,indexes);

                    return Exp.Value(TokenPosition, ((Array)targetObject).GetValue(indexes), returnType);
                }
            }
            else
            {
                DefaultMemberAttribute[] att = targetType.Inspector().GetCustomAttributes<DefaultMemberAttribute>(true);

                MethodInfo methodInfo = targetType.Inspector().GetPropertyGetter(att[0].MemberName, parameterTypes);

                object value = methodInfo.Invoke(targetObject, parameterValues);

                return new ValueExpression(TokenPosition, value, methodInfo.ReturnType);
            }
        }

        public ValueExpression Assign(IParserContext context, object newValue)
        {
            return Evaluate(context, true, newValue);
        }

        public override string ToString()
        {
            string[] parameters = _parameters.ConvertAll(expr => expr.ToString());

            return "(" + _target + "[" + String.Join(",", parameters) + "])";
        }
    }


}