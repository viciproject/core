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
using System.Linq;
using Vici.Core.Cache;

namespace Vici.Core.Parser
{
    public abstract class ExpressionParser
    {
        class ExpressionCompiler
        {
            private readonly ExpressionParser _parser;
            private readonly ExpressionToken[] _tokens;
            private int _currentIndex = -1;
            private ExpressionToken _currentToken;

            public ExpressionCompiler(ExpressionParser parser, ExpressionToken[] tokens)
            {
                _parser = parser;
                _tokens = tokens;
            }

            public Expression Compile()
            {
                CurrentIndex = 0;

                return Compile(multiple: true);
            }

            public ExpressionToken CurrentToken
            {
                get { return _currentToken ?? (_currentToken = (CurrentIndex < _tokens.Length ? _tokens[CurrentIndex] : null)); }
            }

            public int CurrentIndex
            {
                get { return _currentIndex; }
                set { _currentIndex = value; _currentToken = null; }
            }

            public bool MoveNext()
            {
                CurrentIndex++;

                return CurrentIndex < _tokens.Length;
            }

            private Expression CompileStatement()
            {
                return CompileStatement(Int32.MaxValue);
            }

            private Expression CompileStatement(int lastToken)
            {
                RPNExpression rpn = new RPNExpression(_parser.FunctionEvaluator);

                rpn.Start();

                while (CurrentToken != null && CurrentIndex <= lastToken)
                {
                    if (CurrentToken.IsStatementSeperator)
                    {
                        MoveNext();
                        break;
                    }

                    rpn.ApplyToken(CurrentToken);

                    MoveNext();
                }

                rpn.Finish();

                return rpn.Compile();
            }

            private Expression CompileBracketed()
            {
                int level = 0;

                if (!CurrentToken.IsLeftParen)
                    throw new LexerException("Expected (", TokenPosition.Unknown, CurrentToken.Text);

                MoveNext();

                int start = CurrentIndex;

                while(CurrentToken != null)
                {
                    if (CurrentToken.IsRightParen)
                    {
                        if (level > 0)
                        {
                            level--;
                        }
                        else
                        {
                            int idx = CurrentIndex-1;
                            CurrentIndex = start;

                            var expr = CompileStatement(idx);

                            MoveNext();

                            return expr;
                        }
                    }

                    if (CurrentToken.IsLeftParen)
                        level++;

                    MoveNext();
                }

                throw new LexerException("Unterminated foreach() expression", TokenPosition.Unknown, null);
            }

            private Expression Compile(bool multiple)
            {
                if (CurrentToken == null)
                    return null;

                List<Expression> expressions = new List<Expression>();

                bool braced = CurrentToken.IsOpenBrace;

                if (braced)
                {
                    MoveNext();
                    multiple = true;
                }

                IfExpression ifExpression = null;

                while (CurrentToken != null)
                {
                    var token = CurrentToken;

                    switch (token.TokenType)
                    {
                        case TokenType.CloseBrace:
                            {
                                if (!braced)
                                    throw new LexerException(TokenPosition.Unknown, token.Text);

                                MoveNext();
                                multiple = false;
                                break;
                            }
                        case TokenType.ForEach:
                            {
                                MoveNext();

                                InExpression expression = CompileBracketed() as InExpression;

                                if (expression == null)
                                    throw new LexerException("foreach syntax error", token.TokenPosition, token.Text);

                                ForEachExpression forEach = new ForEachExpression(TokenPosition.Unknown);

                                forEach.Iterator = expression.Variable;
                                forEach.Expression = expression.Expression;
                                forEach.Body = Compile(false);

                                expressions.Add(forEach);

                                break;
                            }

                        case TokenType.If:
                            {
                                MoveNext();

                                var expr = CompileBracketed();

                                ifExpression = new IfExpression(TokenPosition.Unknown,expr);

                                ifExpression.TrueExpression = Compile(false);

                                expressions.Add(ifExpression);

                                break;
                            }

                        case TokenType.Else:
                            {
                                MoveNext();

                                if (ifExpression != null)
                                {
                                    ifExpression.FalseExpression = Compile(false);
                                    ifExpression = null;
                                }

                                break;
                            }

                        case TokenType.Return:
                            {
                                MoveNext();

                                Expression expression = Compile(multiple: false);

                                expressions.Add(new ReturnExpression(TokenPosition.Unknown, expression));

                                break;
                            }
                        case TokenType.FunctionDefinition:
                            {
                                MoveNext();

                                var functionExpression = new FunctionDefinitionExpression(TokenPosition.Unknown);

                                if (CurrentToken.TokenType == TokenType.Term)
                                {
                                    functionExpression.Name = CurrentToken.Text;
                                    MoveNext();
                                }

                                int level = 0;
                                int start = CurrentIndex-1;
                                int end = -1;

                                while(CurrentToken != null)
                                {
                                    if (CurrentToken.IsLeftParen)
                                        level++;
                                    else if (CurrentToken.IsRightParen)
                                    {
                                        if (level > 0)
                                        {
                                            level--;

                                            if (level == 0)
                                            {
                                                end = CurrentIndex;
                                                break;
                                            }
                                        }
                                    }

                                    MoveNext();
                                }

                                CurrentIndex = start;
                                
                                
                                var parameters = CompileStatement(end) as CallExpression;

                                functionExpression.ParameterNames = (from p in parameters.Parameters select ((VariableExpression)p).VarName).ToArray();

                                functionExpression.Body = Compile(false);

                                expressions.Add(functionExpression);
                            }
                            break;
                        default:
                            expressions.Add(CompileStatement());
                            break;
                    }

                    if (!multiple)
                        break;
                }

                if (expressions.Count > 1)
                    return new SequenceExpression(TokenPosition.Unknown, expressions.ToArray());

                if (expressions.Count == 1)
                    return expressions[0];

                return null;
            }

        }

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

            ExpressionToken[] tokens = _tokenizer.Tokenize(s, position).Where(t => t.TokenType != TokenType.WhiteSpace).ToArray();

            expression = new ExpressionCompiler(this, tokens).Compile();

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
