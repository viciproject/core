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
    public class AnyOfMatcher : ITokenMatcher
    {
        private readonly ITokenMatcher[] _matchers;

        public AnyOfMatcher(params ITokenMatcher[] matchers)
        {
            _matchers = matchers;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            ITokenProcessor[] tokenProcessors = new ITokenProcessor[_matchers.Length];

            for (int i = 0; i < _matchers.Length; i++)
                tokenProcessors[i] = _matchers[i].CreateTokenProcessor();

            return new MatchProcessor(tokenProcessors);
        }

        private class MatchProcessor : ITokenProcessor
        {
            private readonly ITokenProcessor[] _tokenProcessors;

            public MatchProcessor(ITokenProcessor[] matchers)
            {
                _tokenProcessors = matchers;
            }

            public void ResetState()
            {
                foreach (ITokenProcessor tokenProcessor in _tokenProcessors)
                    tokenProcessor.ResetState();
            }

            public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
            {
                TokenizerState returnState = TokenizerState.Fail;

                foreach (ITokenProcessor matcher in _tokenProcessors)
                {
                    TokenizerState state = matcher.ProcessChar(c, fullExpression, currentIndex);

                    if (state == TokenizerState.Success)
                        returnState = state;

                    if (state == TokenizerState.Valid && returnState == TokenizerState.Fail)
                        returnState = state;
                }

                return returnState;
            }

        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}