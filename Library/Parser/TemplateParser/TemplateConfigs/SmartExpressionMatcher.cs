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

namespace Vici.Core.Parser.Config
{
    public class SmartExpressionMatcher : ITokenMatcher, ITokenProcessor
    {
        private readonly string _terminationChars;

        private int _parenLevel;
        private int _bracketLevel;
        private char _literalChar;
        private State _state;
        private bool _started;

        public SmartExpressionMatcher(string terminationChars)
        {
            _terminationChars = terminationChars;
        }

        private enum State
        {
            MatchingExpression,
            InEscape,
            InLiteral
        }

        ITokenProcessor ITokenMatcher.CreateTokenProcessor()
        {
            return new SmartExpressionMatcher(_terminationChars);
        }

        void ITokenProcessor.ResetState()
        {
            _parenLevel = _bracketLevel = 0;
            _state = State.MatchingExpression;
            _started = false;
        }

        TokenizerState ITokenProcessor.ProcessChar(char c, string fullExpression, int currentIndex)
        {
            switch (_state)
            {
                case State.MatchingExpression:
                    {
                        if (_parenLevel == 0 && _bracketLevel == 0 && _terminationChars.IndexOf(c) >= 0 || "{}\0".IndexOf(c) >= 0)
                        {
                            if (!_started)
                                return TokenizerState.Fail;

                            return TokenizerState.Success;
                        }
                        else if (c == '"' || c == '\'')
                        {
                            _state = State.InLiteral;
                            _literalChar = c;
                        }
                        else if (c == '(')
                        {
                            _parenLevel++;
                        }
                        else if (c == ')')
                        {
                            _parenLevel--;
                        }
                        else if (c == '[')
                        {
                            _bracketLevel++;
                        }
                        else if (c == ']')
                        {
                            _bracketLevel--;
                        }

                        _started = true;
                    }
                    break;

                case State.InEscape:
                    {
                        _state = State.InLiteral;
                    }
                    break;

                case State.InLiteral:
                    {
                        if (c == '\\')
                            _state = State.InEscape;
                        else if (c == _literalChar)
                            _state = State.MatchingExpression;
                    }
                    break;

            }

            return TokenizerState.Valid;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}