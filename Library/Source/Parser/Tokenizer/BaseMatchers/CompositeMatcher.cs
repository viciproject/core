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
    public class CompositeMatcher : ITokenMatcher
    {
        private readonly ITokenMatcher[] _tokens;

        public CompositeMatcher(params ITokenMatcher[] tokens)
        {
            _tokens = tokens;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            ITokenProcessor[] tokenProcessors = new ITokenProcessor[_tokens.Length];

            for (int i=0;i<_tokens.Length;i++)
                tokenProcessors[i] = _tokens[i].CreateTokenProcessor();

            return new CompositeTokenProcessor(tokenProcessors);
        }

        protected ITokenMatcher[] TokenMatchers
        {
            get { return _tokens; }
        }

        protected class CompositeTokenProcessor : ITokenProcessor
        {
            private readonly ITokenProcessor[] _tokenProcessors;
            private readonly int[] _startIndexes;
            private int _firstIndex;

            private int _current;

            public CompositeTokenProcessor(ITokenProcessor[] tokens)
            {
                _tokenProcessors = tokens;
                _startIndexes = new int[tokens.Length];
            }

            public virtual void ResetState()
            {
                _tokenProcessors[0].ResetState();
                _current = 0;
                _firstIndex = -1;
                _startIndexes[0] = 0;
            }

            public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
            {
                TokenizerState state = _tokenProcessors[_current].ProcessChar(c, fullExpression, currentIndex);

                if (state == TokenizerState.Success)
                {
                    _current++;

                    if (_current == _tokenProcessors.Length)
                        return TokenizerState.Success;

                    _startIndexes[_current] = currentIndex - _firstIndex;
                    _tokenProcessors[_current].ResetState();

                    state = _tokenProcessors[_current].ProcessChar(c, fullExpression, currentIndex);
                }

                if (state == TokenizerState.Fail)
                    return TokenizerState.Fail;

                if (_current == 0 && _firstIndex < 0)
                    _firstIndex = currentIndex;

                return TokenizerState.Valid;
            }

            public ITokenProcessor[] TokenProcessors
            {
                get { return _tokenProcessors; }
            }

            public int[] StartIndexes
            {
                get { return _startIndexes; }
            }
        }

        string ITokenMatcher.TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return TranslateToken(originalToken, (CompositeTokenProcessor) tokenProcessor);
        }

        protected virtual string TranslateToken(string originalToken, CompositeTokenProcessor tokenProcessor)
        {
            return originalToken;
        }

    }
}