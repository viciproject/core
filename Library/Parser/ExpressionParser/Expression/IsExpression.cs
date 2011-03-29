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
    public class IsExpression : Expression
    {
        private readonly Expression _objectExpression;
        private readonly Expression _typeExpression;

        public IsExpression(TokenPosition position, Expression objectExpression, Expression typeExpression) : base(position)
        {
            _objectExpression = objectExpression;
            _typeExpression = typeExpression;
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            ClassName className = _typeExpression.Evaluate(context).Value as ClassName;
            ValueExpression objectValue = _objectExpression.Evaluate(context);
            Type objectType = objectValue.Type;

            if (objectValue.Value == null)
                return Exp.Value(TokenPosition, false);

            objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (className == null)
                throw new ExpressionEvaluationException("is operator requires a type. " + _typeExpression + " is not a type", this);
            
            Type checkType = className.Type;

            if (!objectType.IsValueType)
                return Exp.Value(TokenPosition, checkType.IsAssignableFrom(objectType));

            checkType = Nullable.GetUnderlyingType(checkType) ?? checkType;

            return Exp.Value(TokenPosition, checkType == objectType);
        }

        public override string ToString()
        {
            return "(" + _objectExpression + " is " + _typeExpression + ")";
        }
    }
}
