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
    public class ExpressionToken : Token
    {
        internal int NumTerms { get { return _numTerms; } set { _numTerms = value; }}
        
        public new ExpressionTokenMatcher TokenMatcher { get { return (ExpressionTokenMatcher)base.TokenMatcher; } }

        private int _precedence;
        private TokenType _tokenType;
        private OperatorAssociativity _associativity = OperatorAssociativity.Left;
        private int _numTerms;
        private TokenEvaluator _evaluator;

        public ExpressionToken()
        {
            throw new NotSupportedException();
        }

        protected ExpressionToken(string token) : base(null,token)
        {
        }

        public ExpressionToken(ExpressionTokenMatcher tokenMatcher, string text) : base(tokenMatcher, text)
        {
            switch (tokenMatcher.TokenType)
            {
                case TokenType.TernaryOperator: _numTerms = 3; break;
                case TokenType.UnaryOperator: _numTerms = 1; break;
                case TokenType.Operator: _numTerms = 2; break;
            }

            _precedence = tokenMatcher.Precedence;
            _associativity = tokenMatcher.Associativity;
            _tokenType = tokenMatcher.TokenType;
            _evaluator = tokenMatcher.Evaluator;
        }

        internal TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }

        internal OperatorAssociativity Associativity
        {
            get { return _associativity; }
            set { _associativity = value; }
        }

        internal int Precedence
        {
            get { return _precedence; }
            set { _precedence = value; }
        }

        internal TokenEvaluator Evaluator
        {
            get { return _evaluator; }
            set { _evaluator = value; }
        }

        internal bool IsOperator
        {
            get { return (TokenType == TokenType.Operator) || (TokenType == TokenType.UnaryOperator); }
        }

        internal bool IsTerm
        {
            get { return (TokenType == TokenType.Term); }
        }

        internal bool IsUnary
        {
            get { return (TokenType == TokenType.UnaryOperator); }
        }

        internal bool IsFunction
        {
            get { return (TokenType == TokenType.FunctionCall); }
        }

        internal bool IsLeftParen
        {
            get { return (TokenType == TokenType.LeftParen); }
        }

        internal bool IsRightParen
        {
            get { return (TokenType == TokenType.RightParen); }
        }

        internal bool IsArgumentSeparator
        {
            get { return TokenType == TokenType.ArgumentSeparator; }
        }

        public bool IsPartial
        {
            get { return TokenMatcher != null && TokenMatcher.IsPartial; }
        }

        public ExpressionToken Alternate
        {
            get
            {
                if (Alternates != null)
                    foreach (ExpressionToken token in Alternates)
                        return token;

                return null;
            }
        }

        public ExpressionTokenMatcher Root
        {
            get { return TokenMatcher.Root; }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}