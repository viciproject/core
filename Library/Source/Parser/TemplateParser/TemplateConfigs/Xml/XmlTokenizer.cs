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
using System.Text.RegularExpressions;

namespace Vici.Core.Parser.Config
{
    public class XmlTokenizer: TemplateTokenizer
    {
        private class XmlDollarExpressionMatcher : CompositeMatcher
        {
            public XmlDollarExpressionMatcher()
                : base(
                    new CharMatcher('$'),
                    new SmartExpressionMatcher(" \t\r\n\"'<")
                    )
            {
            }

            protected override string TranslateToken(string originalToken, CompositeTokenProcessor tokenProcessor)
            {
                return originalToken.Substring(1);
            }
        }

        private class ForeachTokenMatcher : WrappedExpressionMatcher
        {
            public ForeachTokenMatcher() : base("<foreach" , ">")
            {
            }

            protected override string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
            {
                string s = base.TranslateToken(originalToken, tokenProcessor);

                Match m = Regex.Match(s, @"^var=(?<q>""|')(?<iterator>[a-z_][a-z0-9_]*)\k<q>\s+in=(?<q>""|')(?<expr>.*?)\k<q>$");

                return m.Groups["iterator"].Value + "\0" + m.Groups["expr"].Value;
            }
        }

        private class IfTokenMatcher : WrappedExpressionMatcher
        {
            public IfTokenMatcher(string tag) : base("<"+tag, ">")
            {
            }

            protected override string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
            {
                string s = base.TranslateToken(originalToken, tokenProcessor);

                Match m = Regex.Match(s, @"^condition=(?<q>""|')(?<condition>.*)\k<q>$");

                return m.Groups["condition"].Value;
            }
        }

        public XmlTokenizer()
        {
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("${","}") );
            AddTokenMatcher(TemplateTokenType.Statement, new WrappedExpressionMatcher("<!--${", "}-->"));
            AddTokenMatcher(TemplateTokenType.Expression, new XmlDollarExpressionMatcher());
            AddTokenMatcher(TemplateTokenType.EndBlock, new StringMatcher("</if>"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, new StringMatcher("</foreach>"), true);
            AddTokenMatcher(TemplateTokenType.Else, new StringMatcher("<else/>"), true);
            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachTokenMatcher(), true);
            AddTokenMatcher(TemplateTokenType.If, new IfTokenMatcher("if"), true);
            AddTokenMatcher(TemplateTokenType.ElseIf, new IfTokenMatcher("elseif"), true);
        }
    }
}