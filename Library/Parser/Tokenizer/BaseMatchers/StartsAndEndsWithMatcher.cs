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
    public class StartsAndEndsWithMatcher : ITokenMatcher
    {
        private readonly string _startString;
        private readonly string _endString;
        private readonly char _embeddedStringChar;

        public StartsAndEndsWithMatcher(string startString, string endString)
        {
            _startString = startString;
            _endString = endString;
            _embeddedStringChar = '\0';
        }

        public StartsAndEndsWithMatcher(string startString, string endString, char embeddedStringChar)
        {
            _startString = startString;
            _endString = endString;
            _embeddedStringChar = embeddedStringChar;
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            return new MatchProcessor(this);
        }

        private class MatchProcessor : ITokenProcessor
        {
            private readonly StartsAndEndsWithMatcher _matcher;

            public MatchProcessor(StartsAndEndsWithMatcher matcher)
            {
                _matcher = matcher;
            }

            private enum State
            {
                MatchingStart,
                InString,
                InEscape,
                MatchingEnd,
                Success
            }

            private State _state;
            private int _index;

            public void ResetState()
            {
                _state = State.MatchingStart;
                _index = 0;
            }

            public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
            {
                switch (_state)
                {
                    case State.MatchingStart:
                        {
                            bool match = _matcher._startString[_index++] == c;

                            if (match)
                            {
                                if (_index == _matcher._startString.Length)
                                {
                                    _state = State.MatchingEnd;
                                    _index = 0;
                                }

                                return TokenizerState.Valid;
                            }
                            else
                            {
                                return TokenizerState.Fail;
                            }
                        }

                    case State.MatchingEnd:
                        {
                            if (c == '\0')
                                return TokenizerState.Fail;

                            if (c == _matcher._embeddedStringChar)
                            {
                                _state = State.InString;

                                return TokenizerState.Valid;
                            }

                            if (c != _matcher._endString[_index++])
                                _index = 0;

                            if (_index == _matcher._endString.Length)
                                _state = State.Success;

                            return TokenizerState.Valid;
                        }

                    case State.InString:
                        {
                            if (c == '\\')
                                _state = State.InEscape;

                            if (c == _matcher._embeddedStringChar)
                            {
                                _state = State.MatchingEnd;
                                _index = 0;
                            }

                            return TokenizerState.Valid;
                        }

                    case State.InEscape:
                        {
                            _state = State.InString;

                            return TokenizerState.Valid;
                        }

                    case State.Success:
                        return TokenizerState.Success;

                    default:
                        return TokenizerState.Valid;
                }
            }

        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}