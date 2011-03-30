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
    public class TemplateTokenMatcher : ITokenMatcher
    {
        private readonly ITokenMatcher _tokenMatcher;
        private readonly bool _removeEmptyLine;
        private readonly string _tokenId;
        private readonly TemplateTokenType _tokenType;

        public TemplateTokenMatcher(ITokenMatcher tokenMatcher, TemplateTokenType tokenType, bool removeEmptyLine, string tokenId)
        {
            _tokenMatcher = tokenMatcher;
            _removeEmptyLine = removeEmptyLine;
            _tokenId = tokenId;
            _tokenType = tokenType;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            return _tokenMatcher.CreateTokenProcessor();
        }

        public TemplateTokenType TokenType
        {
            get { return _tokenType; }
        }

        public string TokenId
        {
            get { return _tokenId; }
        }

        public bool RemoveEmptyLine
        {
            get { return _removeEmptyLine; }
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return _tokenMatcher.TranslateToken(originalToken, tokenProcessor);
        }
    }
}