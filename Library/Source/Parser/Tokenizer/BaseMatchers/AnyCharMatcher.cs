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
    public class AnyCharMatcher : ITokenMatcher, ITokenProcessor
    {
        private bool _seen;
        private readonly bool _caseSensitive;
        private readonly string _chars;

        public AnyCharMatcher(string chars)
        {
            _chars = chars;
            _caseSensitive = true;
        }

        public AnyCharMatcher(string chars, bool caseSensitive)
        {
            _chars = caseSensitive ? chars : chars.ToLowerInvariant();

            _caseSensitive = caseSensitive;
        }

        ITokenProcessor ITokenMatcher.CreateTokenProcessor()
        {
            return new AnyCharMatcher(_chars, _caseSensitive);
        }

        void ITokenProcessor.ResetState()
        {
            _seen = false;
        }

        TokenizerState ITokenProcessor.ProcessChar(char c, string fullExpression, int currentIndex)
        {
            if (_seen)
                return TokenizerState.Success;

            if (_caseSensitive)
            {
                if (_chars.IndexOf(c) < 0)
                    return TokenizerState.Fail;
            }
            else
            {
                if (_chars.IndexOf(char.ToLowerInvariant(c)) < 0)
                    return TokenizerState.Fail;
            }

            _seen = true;

            return TokenizerState.Valid;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}