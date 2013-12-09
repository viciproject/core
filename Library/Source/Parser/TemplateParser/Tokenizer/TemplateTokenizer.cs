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
    public class TemplateTokenizer : Tokenizer<TemplateToken>
    {
        public TemplateTokenizer() : base(true)
        {
        }

        public void AddTokenMatcher(TemplateTokenType templateTokenType, ITokenMatcher tokenMatcher, bool removeEmptyLine, string tokenId)
        {
            AddTokenMatcher(new TemplateTokenMatcher(tokenMatcher, templateTokenType, removeEmptyLine, tokenId));
        }

        public void AddTokenMatcher(TemplateTokenType templateTokenType, ITokenMatcher tokenMatcher)
        {
            AddTokenMatcher(new TemplateTokenMatcher(tokenMatcher, templateTokenType, false, null));
        }

        public void AddTokenMatcher(TemplateTokenType templateTokenType, ITokenMatcher tokenMatcher, bool removeEmptyLine)
        {
            AddTokenMatcher(new TemplateTokenMatcher(tokenMatcher, templateTokenType, removeEmptyLine, null));
        }
   
        public void AddTokenMatcher(TemplateTokenType templateTokenType, ITokenMatcher tokenMatcher, string tokenId)
        {
            AddTokenMatcher(new TemplateTokenMatcher(tokenMatcher, templateTokenType, false, tokenId));
        }

        public override TemplateToken CreateToken(ITokenMatcher tokenMatcher, string token)
        {
            TemplateTokenMatcher matcher = (TemplateTokenMatcher) tokenMatcher;

            if (matcher != null && matcher.TokenType == TemplateTokenType.ForEach)
            {
                string[] pieces = token.Split('\0');

                return new ForeachTemplateToken(matcher, pieces[0], pieces[1]);
            }
            else
                return new TemplateToken(matcher, token);
        }
    }
}