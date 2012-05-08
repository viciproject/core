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

namespace Vici.Core.Parser
{
    public class ExpressionTokenizer : Tokenizer<ExpressionToken>
    {
        public void AddTokenMatcher(ITokenMatcher tokenMatcher, TokenType tokenType)
        {
            AddTokenMatcher(tokenMatcher, tokenType, 0, OperatorAssociativity.Left, null);
        }

        public void AddTokenMatcher(ITokenMatcher tokenMatcher, TokenType tokenType, TokenEvaluator tokenEvaluator)
        {
            AddTokenMatcher(tokenMatcher, tokenType, 0, OperatorAssociativity.Left, tokenEvaluator);
        }

        public void AddTokenMatcher(ITokenMatcher tokenMatcher, TokenType tokenType, int precedence, TokenEvaluator tokenEvaluator)
        {
            AddTokenMatcher(tokenMatcher, tokenType, precedence, OperatorAssociativity.Left, tokenEvaluator);
        }

        public void AddTokenMatcher(ITokenMatcher tokenMatcher, TokenType tokenType, int precedence, OperatorAssociativity associativity, TokenEvaluator tokenEvaluator)
        {
            AddTokenMatcher(new ExpressionTokenMatcher(tokenMatcher, tokenType, precedence, associativity, tokenEvaluator));
        }

        public void AddTernaryTokenMatcher(ITokenMatcher matcher1, ITokenMatcher matcher2, int precedence, OperatorAssociativity associativity, TokenEvaluator tokenEvaluator)
        {
            ExpressionTokenMatcher root = new ExpressionTokenMatcher(null, TokenType.TernaryOperator, tokenEvaluator);

            ExpressionTokenMatcher partial1 = new ExpressionTokenMatcher(matcher1, TokenType.TernaryOperator1, precedence, associativity, null);
            ExpressionTokenMatcher partial2 = new ExpressionTokenMatcher(matcher2, TokenType.TernaryOperator1, precedence, associativity, null);

            partial1.Root = root;
            partial2.Root = root;

            AddTokenMatcher(partial1);
            AddTokenMatcher(partial2);
        }

        public override ExpressionToken CreateToken(ITokenMatcher tokenMatcher, string token)
        {
            ExpressionToken t = new ExpressionToken((ExpressionTokenMatcher) tokenMatcher, token);

            

            return t;
        }

        //        public void AddTernaryOperator(string pattern1, string pattern2, int precedence, OperatorAssociativity associativity, TokenEvaluator evaluator)
        //        {
        //            TokenDefinition root = new TokenDefinition(TokenType.TernaryOperator, evaluator);
        //
        //            TokenDefinition partial1 = new TokenDefinition(TokenType.TernaryOperator1, precedence, associativity, pattern1);
        //            TokenDefinition partial2 = new TokenDefinition(TokenType.TernaryOperator2, precedence, associativity, pattern2);
        //
        //            partial1.Root = root;
        //            partial2.Root = root;
        //
        //            AddTokenDefinition(partial1);
        //            AddTokenDefinition(partial2);
        //        }

    }
}