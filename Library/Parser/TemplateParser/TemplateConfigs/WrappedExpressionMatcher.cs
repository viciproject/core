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
    public class WrappedExpressionMatcher : ITokenMatcher, ITokenProcessor
    {
        private readonly bool _caseSensitive;
        private readonly string[] _parts;
        private int _index;
        private State _state;
        private int _parenLevel;
        private int _bracketLevel;
        private char _literalChar;

        private int _currentPart;

        private int _startIndex;
        private string _expression;

        public WrappedExpressionMatcher(bool caseSensitive, params string[] parts)
        {
            _caseSensitive = caseSensitive;
            _parts = parts;
        }

        public WrappedExpressionMatcher(params string[] parts)
        {
            _caseSensitive = true;
            _parts = parts;
        }

        public virtual ITokenProcessor CreateTokenProcessor()
        {
            return new WrappedExpressionMatcher(_parts);
        }

        private enum State
        {
            MatchingPart,
            MatchingExpression,
            InLiteral,
            InEscape,
            Success
        }

        void ITokenProcessor.ResetState()
        {
            _state = State.MatchingPart;
            _index = 0;
            _currentPart = 0;

            _startIndex = -1;
        }

        private string Expression
        {
            get { return _expression; }
        }

        private bool IsMatch(char c1, char c2)
        {
            return CharHelper.IsMatch(c1, c2, _caseSensitive);
        }

        public virtual TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
        {
            switch (_state)
            {
                case State.MatchingPart:
                    {
                        if (_index == 0 && _currentPart > 0 && " \t\n\r".IndexOf(c) >= 0)
                            return TokenizerState.Valid;

                        bool match = IsMatch(_parts[_currentPart][_index++], c);

                        if (!match)
                            return TokenizerState.Fail;

                        if (_index == _parts[_currentPart].Length)
                        {
                            _currentPart++;
                            _index = 0;

                            if (_currentPart == _parts.Length - 1)
                            {
                                _startIndex = currentIndex + 1;

                                _state = State.MatchingExpression;
                            }
                        }
                    }
                    break;

                case State.MatchingExpression:
                    {
                        if (c == '\0')
                            return TokenizerState.Fail;

                        if (_bracketLevel == 0 && _parenLevel == 0)
                        {
                            if (!IsMatch(c, _parts[_parts.Length - 1][_index++]))
                            {
                                _index = 0;
                            }
                            else if (_index == _parts[_parts.Length - 1].Length)
                            {
                                _expression = fullExpression.Substring(_startIndex, currentIndex - _startIndex - _index + 1);

                                _state = State.Success;
                            }
                        }
                        else
                        {
                            _index = 0;
                        }

                        if (c == '"' || c == '\'')
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
                            if (_parenLevel > 0)
                                _parenLevel--;
                        }
                        else if (c == '[')
                        {
                            _bracketLevel++;
                        }
                        else if (c == ']')
                        {
                            if (_bracketLevel > 0)
                                _bracketLevel--;
                        }
                    }
                    break;

                case State.InLiteral:
                    {
                        if (c == '\\')
                        {
                            _state = State.InEscape;
                        }
                        else if (c == _literalChar)
                        {
                            _state = State.MatchingExpression;
                            _index = 0;
                        }
                    }
                    break;

                case State.InEscape:
                    {
                        _state = State.InLiteral;
                    }
                    break;

                case State.Success:
                    return TokenizerState.Success;
            }

            return TokenizerState.Valid;
        }

        string ITokenMatcher.TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return TranslateToken(originalToken, (WrappedExpressionMatcher) tokenProcessor);
        }

        protected virtual string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
        {
            return tokenProcessor.Expression.Trim();
        }
    }
}