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
    //TODO: split tokenprocessor to avoid overhead of array sorting
    public class AnyOfStringMatcher : ITokenMatcher, ITokenProcessor
    {
        private int _index;
        private int _lastMatch;
        private readonly string[] _strings;

        public AnyOfStringMatcher(params string[] strings)
        {
            _strings = strings;

            Array.Sort(_strings, (s1, s2) => s1.Length - s2.Length);
        }

        public ITokenProcessor CreateTokenProcessor()
        {
            return new AnyOfStringMatcher(_strings);
        }

        public void ResetState()
        {
            _index = 0;
            _lastMatch = -1;
        }

        public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
        {
            for (int i=0;i<_strings.Length;i++)
                if (_index < _strings[i].Length && _strings[i][_index] == c)
                {
                    _lastMatch = i;
                    _index++;

                    return TokenizerState.Valid;
                }

            if (_lastMatch >= 0 && _index >= _strings[_lastMatch].Length)
                return TokenizerState.Success;

            return TokenizerState.Fail;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}