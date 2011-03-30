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

namespace Vici.Core.Parser
{
    public abstract class Expression : IExpression
    {
        public TokenPosition TokenPosition { get; private set; }

        protected Expression(TokenPosition tokenPosition)
        {
            TokenPosition = tokenPosition;
        }

        public abstract ValueExpression Evaluate(IParserContext context);

        protected static ValueExpression[] EvaluateExpressionArray(Expression[] expressions, IParserContext context)
        {
            return expressions.ConvertAll(expr => expr.Evaluate(context));
        }

    	object IExpression.EvaluateToObject(IParserContext context)
    	{
    		return Evaluate(context).Value;
    	}

    	IValueWithType IExpression.Evaluate(IParserContext context)
    	{
    		ValueExpression value = Evaluate(context);

            return new ValueExpression(TokenPosition, value.Value,value.Type);
    	}

    	public T Evaluate<T>(IParserContext context)
    	{
    		return (T) Evaluate(context).Value;
    	}
    }

    public static class Exp
    {
        public static AddExpression Add(TokenPosition position, Expression left, Expression right) { return new AddExpression(position, left, right); }
        public static SubtractExpression Subtract(TokenPosition position, Expression left, Expression right) { return new SubtractExpression(position, left, right); }
        public static MultiplyExpression Multiply(TokenPosition position, Expression left, Expression right) { return new MultiplyExpression(position, left, right); }
        public static DivideExpression Divide(TokenPosition position, Expression left, Expression right) { return new DivideExpression(position, left, right); }
        public static ValueExpression<T> Value<T>(TokenPosition position, T value) { return new ValueExpression<T>(position, value); }
        public static ValueExpression Value(TokenPosition position, object value, Type type) { return new ValueExpression(position, value, type); }
        public static BinaryArithmicExpression Op(TokenPosition position, string op, Expression left, Expression right) { return new BinaryArithmicExpression(position, op, left, right); }
        public static AndAlsoExpression AndAlso(TokenPosition position, Expression left, Expression right) { return new AndAlsoExpression(position, left, right); }
        public static OrElseExpression OrElse(TokenPosition position, Expression left, Expression right) { return new OrElseExpression(position, left, right); }
    }
}
