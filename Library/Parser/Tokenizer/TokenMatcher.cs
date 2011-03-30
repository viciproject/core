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
    internal class TokenMatcher
    {
        private bool _stillValid = true;

        private readonly ITokenProcessor _tokenProcessor;
        private readonly ITokenMatcher _tokenMatcher;

        public TokenMatcher(ITokenMatcher tokenMatcher)
        {
            _tokenProcessor = tokenMatcher.CreateTokenProcessor();
            _tokenMatcher = tokenMatcher;
        }

        public ITokenMatcher Matcher
        {
            get { return _tokenMatcher; }
        }

        public void Reset()
        {
            _stillValid = true;

            _tokenProcessor.ResetState();
        }

        public TokenizerState Feed(char c, string fullExpression, int currentIndex)
        {
            if (!_stillValid)
                return TokenizerState.Fail;

            TokenizerState state = _tokenProcessor.ProcessChar(c, fullExpression, currentIndex);

            if (state != TokenizerState.Valid)
                _stillValid = false;

            return state;
        }


        public override string ToString()
        {
            string s = "";
            
            if (_stillValid)
                s += "StillValid/";

            s += _tokenProcessor == null ? "(null)" : _tokenProcessor.GetType().Name;

            return s;
        }

        public string TranslateToken(string text)
        {
            return _tokenMatcher.TranslateToken(text, _tokenProcessor);
        }
    }
}