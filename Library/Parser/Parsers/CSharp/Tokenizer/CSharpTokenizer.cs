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

namespace Vici.Core.Parser
{
    public class CSharpTokenizer : ExpressionTokenizer
    {
        public CSharpTokenizer(bool allowScripting)
        {
            AddTokenMatcher(new CharMatcher('('), TokenType.LeftParen);
            AddTokenMatcher(new CharMatcher(')'), TokenType.RightParen);
            AddTokenMatcher(new CharMatcher('['), TokenType.LeftParen);
            AddTokenMatcher(new CharMatcher(']'), TokenType.RightParen);
            AddTokenMatcher(new CharMatcher(','), TokenType.ArgumentSeparator);
            AddTokenMatcher(new StringMatcher("&&"), TokenType.Operator, 10, CSharpEvaluator.ShortcutOperator);
            AddTokenMatcher(new StringMatcher("||"), TokenType.Operator, 9, CSharpEvaluator.ShortcutOperator);
            AddTokenMatcher(new StringMatcher("??"), TokenType.Operator, 8, CSharpEvaluator.Coalesce);
            AddTokenMatcher(new StringMatcher("?:"), TokenType.Operator, 8, CSharpEvaluator.DefaultValueOperator);
            AddTernaryTokenMatcher(new CharMatcher('?'), new CharMatcher(':'), 7, OperatorAssociativity.Right, CSharpEvaluator.Ternary);
            AddTokenMatcher(new AnyOfStringMatcher("==","!="), TokenType.Operator, 14,CSharpEvaluator.Operator);
            AddTokenMatcher(new CharMatcher('.'), TokenType.Operator, 20,CSharpEvaluator.DotOperator);
            AddTokenMatcher(new AnyCharMatcher("!-~"), TokenType.UnaryOperator, 19, OperatorAssociativity.Right, CSharpEvaluator.Unary);
            AddTokenMatcher(new AnyCharMatcher("*/%"), TokenType.Operator, 18,CSharpEvaluator.Operator);
            AddTokenMatcher(new AnyCharMatcher("+-"), TokenType.Operator, 17,CSharpEvaluator.Operator);
            AddTokenMatcher(new AnyOfStringMatcher("<<",">>"), TokenType.Operator, 16,CSharpEvaluator.Operator);
            AddTokenMatcher(new AnyOfStringMatcher("<=",">=","<",">"), TokenType.Operator, 15,CSharpEvaluator.Operator);
            AddTokenMatcher(new AnyOfStringMatcher("as","is"), TokenType.Operator, 15,CSharpEvaluator.IsAsOperator);
            
            if (allowScripting)
            {
                AddTokenMatcher(new StringMatcher("foreach"), TokenType.ForEach);
                AddTokenMatcher(new StringMatcher("if"), TokenType.If);
                AddTokenMatcher(new StringMatcher("else"), TokenType.Else);
                AddTokenMatcher(new StringMatcher("return"), TokenType.Return);
                AddTokenMatcher(new StringMatcher("function"), TokenType.FunctionDefinition);
                AddTokenMatcher(new StringMatcher("in"), TokenType.Operator, 2, CSharpEvaluator.InOperator);
            }

            AddTokenMatcher(new AnyCharMatcher("&|"), TokenType.Operator, 13,CSharpEvaluator.Operator);
            AddTokenMatcher(new CharMatcher('^'), TokenType.Operator, 12, CSharpEvaluator.Operator);
            AddTokenMatcher(new CharMatcher('='), TokenType.Operator, 6, OperatorAssociativity.Right, CSharpEvaluator.Assignment);
            AddTokenMatcher(new StringMatcher("..."), TokenType.Operator, 1, CSharpEvaluator.NumericRange);
            AddTokenMatcher(new CompositeMatcher(new StringMatcher("new"), new WhiteSpaceMatcher(), new VariableMatcher()), TokenType.Term, CSharpEvaluator.Constructor);
            AddTokenMatcher(new StringMatcher("typeof"), TokenType.Term, CSharpEvaluator.TypeOf);
            AddTokenMatcher(new VariableMatcher(), TokenType.Term, CSharpEvaluator.VarName);
            AddTokenMatcher(new StringLiteralMatcher(), TokenType.Term, CSharpEvaluator.StringLiteral);
            AddTokenMatcher(new CharLiteralMatcher(), TokenType.Term, CSharpEvaluator.CharLiteral);
            AddTokenMatcher(new WhiteSpaceMatcher(), TokenType.WhiteSpace);
            AddTokenMatcher(new IntegerLiteralMatcher(), TokenType.Term, CSharpEvaluator.Number);
            AddTokenMatcher(new DecimalLiteralMatcher(), TokenType.Term, CSharpEvaluator.Number);
            AddTokenMatcher(new TypeCastMatcher(), TokenType.UnaryOperator, 19, OperatorAssociativity.Right, CSharpEvaluator.TypeCast);

            if (allowScripting)
            {
                AddTokenMatcher(new CharMatcher(';'), TokenType.StatementSeparator);
                AddTokenMatcher(new CharMatcher('{'), TokenType.OpenBrace);
                AddTokenMatcher(new CharMatcher('}'), TokenType.CloseBrace);
            }
        }
    }
}
