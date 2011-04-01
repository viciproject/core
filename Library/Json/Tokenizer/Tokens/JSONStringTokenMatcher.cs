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
using System.Globalization;
using System.Text.RegularExpressions;
using Vici.Core.Parser;

namespace Vici.Core.Json
{
    public class StringTokenMatcher : ITokenMatcher, ITokenProcessor
    {
        private bool _inString;
        private char _stringChar;
        private bool _inEscape;
        private bool _done;

        public ITokenProcessor CreateTokenProcessor()
        {
            return new StringTokenMatcher();
        }

        public void ResetState()
        {
            _inString = false;
            _inEscape = false;
            _done = false;
        }

        public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
        {
            if (_done)
                return TokenizerState.Success;

            if (!_inString)
            {
                if (c == '"' || c == '\'')
                {
                    _inString = true;
                    _stringChar = c;

                    return TokenizerState.Valid;
                }

                return TokenizerState.Fail;
            }

            if (_inEscape)
            {
                _inEscape = false;

                return TokenizerState.Valid;
            }

            if (c == '\\')
            {
                _inEscape = true;

                return TokenizerState.Valid;
            }

            if (c == _stringChar)
            {
                _done = true;

                return TokenizerState.Valid;
            }

            return TokenizerState.Valid;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            if (originalToken.IndexOf('\\') < 0)
                return originalToken;

            string token = originalToken;

            token = token.Replace("\\n", "\n");
            token = token.Replace("\\r", "\r");
            token = token.Replace("\\t", "\t");
            token = token.Replace("\\\"", "\"");

            if (token.IndexOf("\\u") >= 0)
            {
                token = Regex.Replace(token, @"\\[uU][a-fA-F0-9]{4}", m => ((char)uint.Parse(m.Value.Substring(2), NumberStyles.HexNumber)).ToString());
            }

            token = token.Replace("\\\\", "\\");

            return token;
        }
    }
}