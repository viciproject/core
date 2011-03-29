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

namespace Vici.Core.Parser
{
    public class ExpressionTokenMatcher : ITokenMatcher
    {
        private readonly ITokenMatcher _matcher;
        private TokenType _tokenType;
        private TokenEvaluator _tokenEvaluator;
        private int _precedence;
        private OperatorAssociativity _associativity;
        private ExpressionTokenMatcher _root;

        public ExpressionTokenMatcher(ITokenMatcher matcher, TokenType tokenType, TokenEvaluator tokenEvaluator)
        {
            _matcher = matcher;
            _tokenType = tokenType;
            _tokenEvaluator = tokenEvaluator;
        }

        public ExpressionTokenMatcher(ITokenMatcher matcher, TokenType tokenType, int precedence, OperatorAssociativity associativity, TokenEvaluator tokenEvaluator)
        {
            _matcher = matcher;
            _tokenType = tokenType;
            _tokenEvaluator = tokenEvaluator;
            _precedence = precedence;
            _associativity = associativity;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            return _matcher.CreateTokenProcessor();
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return _matcher.TranslateToken(originalToken, tokenProcessor);
        }

        public TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }

        public TokenEvaluator Evaluator
        {
            get { return _tokenEvaluator; }
            set { _tokenEvaluator = value; }
        }

        public int Precedence
        {
            get { return _precedence; }
            set { _precedence = value; }
        }

        public bool IsPartial
        {
            get { return _root != null; }
        }

        public ExpressionTokenMatcher Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public OperatorAssociativity Associativity
        {
            get { return _associativity; }
            set { _associativity = value; }
        }
    }
}