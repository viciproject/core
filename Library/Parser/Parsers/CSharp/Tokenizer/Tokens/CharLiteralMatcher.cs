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
    public class CharLiteralMatcher : ITokenMatcher, ITokenProcessor
    {
        private bool _started;
        private bool _inEscape;
        private bool _done;

        public void ResetState()
        {
            _started = false;
            _inEscape = false;
            _done = false;
        }

        ITokenProcessor ITokenMatcher.CreateTokenProcessor()
        {
            return new CharLiteralMatcher();
        }

        TokenizerState ITokenProcessor.ProcessChar(char c, string fullExpression, int currentIndex)
        {
            if (_done)
                return TokenizerState.Success;

            if (!_started)
            {
                if (c != '\'')
                    return TokenizerState.Fail;

                _started = true;
            }
            else
            {
                if (_inEscape)
                    _inEscape = false;
                else if (c == '\'')
                    _done = true;
                else if (c == '\\')
                    _inEscape = true;
            }

            return TokenizerState.Valid;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}