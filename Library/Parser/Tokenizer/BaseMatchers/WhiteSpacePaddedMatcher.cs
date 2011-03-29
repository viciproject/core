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
    public class WhiteSpacePaddedMatcher : ITokenMatcher
    {
        private readonly ITokenMatcher _matcher;

        public WhiteSpacePaddedMatcher(ITokenMatcher matcher)
        {
            _matcher = matcher;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            return new MatchProcessor(_matcher);
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken.Substring(((MatchProcessor) tokenProcessor).Skipped);
        }

        private class MatchProcessor : ITokenProcessor
        {
            private bool _passedWhitespace;
            private readonly ITokenProcessor _processor;
            private int _skipped;

            public MatchProcessor(ITokenMatcher matcher)
            {
                _processor = matcher.CreateTokenProcessor();
            }

            public int Skipped
            {
                get { return _skipped; }
            }

            public void ResetState()
            {
                _passedWhitespace = false;
                _skipped = 0;

                _processor.ResetState();
            }

            public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
            {
                if (!_passedWhitespace && " \t\r\n".IndexOf(c) >= 0)
                {
                    _skipped = Skipped + 1;
                    return TokenizerState.Valid;
                }

                _passedWhitespace = true;

                return _processor.ProcessChar(c,fullExpression,currentIndex);
            }
        }
    }
}