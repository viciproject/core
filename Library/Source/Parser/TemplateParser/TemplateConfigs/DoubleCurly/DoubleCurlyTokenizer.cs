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

namespace Vici.Core.Parser.Config
{
    public class DoubleCurlyTokenizer : TemplateTokenizer
    {
        private class ForeachTokenMatcher : WrappedExpressionMatcher
        {
            public ForeachTokenMatcher() : base("{{", "foreach", "}}")
            {
            }

            protected override string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
            {
                string s = base.TranslateToken(originalToken, tokenProcessor);

                int inIdx = s.IndexOf(" in ");

                if (inIdx < 0)
                    throw new TemplateParsingException("invalid syntax in foreach",TokenPosition.Unknown);
                else
                    return s.Substring(0, inIdx).Trim() + "\0" + s.Substring(inIdx + 4).Trim();
            }    
        }

        public DoubleCurlyTokenizer()
        {
            AddTokenMatcher(TemplateTokenType.MacroDefinition, new WrappedExpressionMatcher("{{", "macro", "}}"), true);
            AddTokenMatcher(TemplateTokenType.MacroCall, new WrappedExpressionMatcher("{{", "call", "}}"));
            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachTokenMatcher(), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, new WrappedExpressionMatcher("{{", "end", "}}"), true);
            AddTokenMatcher(TemplateTokenType.If, new WrappedExpressionMatcher("{{", "if", "}}"), true);
            AddTokenMatcher(TemplateTokenType.ElseIf, new WrappedExpressionMatcher("{{", "elseif", "}}"), true);
            AddTokenMatcher(TemplateTokenType.Else, new WrappedExpressionMatcher("{{", "else", "}}"),true);
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("{{", "}}"));
            AddTokenMatcher(TemplateTokenType.Comment, new WrappedExpressionMatcher("{#", "#}"), true);
        }
    }
}
