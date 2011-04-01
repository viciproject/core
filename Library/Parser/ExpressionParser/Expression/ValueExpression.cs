#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2011 Philippe Leybaert
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

namespace Vici.Core.Parser
{
    public class ValueExpression : Expression , IValueWithType
    {
        private Type _type;
        private object _value;

        public ValueExpression(TokenPosition position, object value, Type type) : base(position)
        {
            Value = value;
            Type = type;

            if (Type == typeof(object) && Value != null)
                Type = Value.GetType();
        }

        public Type Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            return this;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ValueExpression<T> : ValueExpression
    {
        public ValueExpression(TokenPosition position, T value)
            : base(position, value, typeof(T))
        {
        }

        public new T Value
        {
            get { return (T)base.Value; }
        }
    }


}