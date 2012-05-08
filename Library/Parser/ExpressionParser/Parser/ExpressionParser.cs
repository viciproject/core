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
using Vici.Core.Cache;

namespace Vici.Core.Parser
{
    public abstract class ExpressionParser
    {
        private TokenEvaluator _functionEvaluator;
        private IParserContext _defaultContext;

        private readonly ExpressionTokenizer _tokenizer;

        private readonly SmartCache<IExpression> _expressionCache = new SmartCache<IExpression>(1000);

        protected ExpressionParser(ExpressionTokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public TokenEvaluator FunctionEvaluator
        {
            get { return _functionEvaluator; }
            set { _functionEvaluator = value; }
        }

        public IParserContext DefaultContext
        {
            get { return _defaultContext; }
            set { _defaultContext = value; }
        }

        private RPNExpression ParseToRPN(string s, TokenPosition position)
        {
            RPNExpression rpnExpression = new RPNExpression(_functionEvaluator);

            rpnExpression.Start();

            ExpressionToken[] tokens = _tokenizer.Tokenize(s, position);

            foreach (ExpressionToken token in tokens)
            {
                if (token.TokenType != TokenType.WhiteSpace)
                    rpnExpression.ApplyToken(token);
            }

            rpnExpression.Finish();

            return rpnExpression;
        }

        public void ResetCache()
        {
            _expressionCache.ClearCache();
        }

        public int CacheSize
        {
            get { return _expressionCache.CacheSize; }
            set { _expressionCache.CacheSize = value; }
        }

        public IExpression Parse(string s)
        {
            return Parse(s, TokenPosition.Unknown);
        }

        public IExpression Parse(string s, TokenPosition position)
        {
            IExpression expression;
          
            if (_expressionCache.TryGetValue(s, out expression))
                return expression;

            expression = ParseToRPN(s,position).Compile();

            _expressionCache.Add(s, expression);

            return expression;
        }

        public IExpressionWithContext ParseWithContext(string s, IParserContext context)
        {
            return new ExpressionWithContext(Parse(s), context);
        }

        public IExpressionWithContext ParseWithContext(string s, IParserContext context, TokenPosition position)
        {
            return new ExpressionWithContext(Parse(s, position), context);
        }

        public IExpressionWithContext ParseWithContext(string s)
        {
            return new ExpressionWithContext(Parse(s), _defaultContext);
        }

        public IExpressionWithContext ParseWithContext(string s, TokenPosition position)
        {
            return new ExpressionWithContext(Parse(s, position), _defaultContext);
        }

        public object EvaluateToObject(string s)
        {
            return ParseWithContext(s).EvaluateToObject();
        }

        public object Evaluate(string s, out Type type)
        {
            IValueWithType value = ParseWithContext(s).Evaluate();

            type = value.Type;

            return value.Value;
        }

        public IValueWithType Evaluate(string s)
        {
            return ParseWithContext(s).Evaluate();
        }

        public T Evaluate<T>(string s)
        {
            return ParseWithContext(s).Evaluate<T>();
        }

        public T Evaluate<T>(string s, TokenPosition tokenPosition)
        {
            return ParseWithContext(s, tokenPosition).Evaluate<T>();
        }

        public IValueWithType Evaluate(string s, IParserContext context)
        {
            return Evaluate(s, context, TokenPosition.Unknown);
        }

        public IValueWithType Evaluate(string s, IParserContext context, TokenPosition tokenPosition)
        {
            return ParseWithContext(s, context, tokenPosition).Evaluate();
        }

        public object EvaluateToObject(string s, IParserContext context)
        {
            return ParseWithContext(s, context).EvaluateToObject();
        }

        public object EvaluateToObject(string s, IParserContext context, TokenPosition tokenPosition)
        {
            return ParseWithContext(s, context, tokenPosition).EvaluateToObject();
        }

        public object Evaluate(string s, out Type type, IParserContext context)
        {
            return Evaluate(s, out type, context, TokenPosition.Unknown);
        }

        public object Evaluate(string s, out Type type, IParserContext context, TokenPosition tokenPosition)
        {
            IValueWithType value = ParseWithContext(s, context, tokenPosition).Evaluate();

            type = value.Type;

            return value.Value;
        }

        public T Evaluate<T>(string s, IParserContext context)
        {
            return ParseWithContext(s, context).Evaluate<T>();
        }

        public T Evaluate<T>(string s, IParserContext context, TokenPosition tokenPosition)
        {
            return ParseWithContext(s, context, tokenPosition).Evaluate<T>();
        }
    }
}
