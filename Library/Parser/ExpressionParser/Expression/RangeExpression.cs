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
    public class RangeExpression : Expression
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public RangeExpression(TokenPosition tokenPosition, Expression from, Expression to) : base(tokenPosition)
        {
            _from = from;
            _to = to;
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            ValueExpression from = _from.Evaluate(context);
            ValueExpression to = _to.Evaluate(context);

            if (from.Type != typeof(int) && from.Type != typeof(long))
                throw new ExpressionEvaluationException("Expression " + from + " does not evaluate to int or long", from);

            if (to.Type != typeof(int) && to.Type != typeof(long))
                throw new ExpressionEvaluationException("Expression " + from + " does not evaluate to int or long", from);

            if (from.Type == typeof(long) || to.Type == typeof(long))
                return Exp.Value(TokenPosition, Range((long)Convert.ChangeType(from.Value, typeof(long), null), (long)Convert.ChangeType(to.Value, typeof(long), null)));
            else
                return Exp.Value(TokenPosition, Range((int)Convert.ChangeType(from.Value, typeof(int), null), (int)Convert.ChangeType(to.Value, typeof(int), null)));
        }

        public static IEnumerable<int> Range(int from, int to)
        {
            if (from == to)
                yield return from;
            else if (from < to)
                for (int i = from; i <= to; i++)
                    yield return i;
            else
                for (int i = from; i >= to; i--)
                    yield return i;
        }

        public static IEnumerable<long> Range(long from, long to)
        {
            if (from == to)
                yield return from;
            else if (from < to)
                for (long i = from; i <= to; i++)
                    yield return i;
            else
                for (long i = from; i >= to; i--)
                    yield return i;
        }

    }
}