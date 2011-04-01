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
    public class TypeCastMatcher : ITokenMatcher, ITokenProcessor
    {
        private bool _sawStart;
        private bool _sawEnd;
        private bool _sawType;

        public void ResetState()
        {
            _sawStart = false;
            _sawEnd = false;
            _sawType = false;
        }

        ITokenProcessor ITokenMatcher.CreateTokenProcessor()
        {
            return new TypeCastMatcher();
        }

        TokenizerState ITokenProcessor.ProcessChar(char c, string fullExpression, int currentIndex)
        {
            if (_sawEnd)
            {

                return TokenizerState.Success;
            }

            if (!_sawStart)
            {
                if (c != '(')
                    return TokenizerState.Fail;
                
                _sawStart = true;
                
                return TokenizerState.Valid;
            }

            if (!_sawType)
            {
                if (char.IsWhiteSpace(c))
                    return TokenizerState.Valid;
                
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '.' || c == '_')
                {
                    _sawType = true;
                    return TokenizerState.Valid;
                }

                return TokenizerState.Fail;
            }

            if (char.IsWhiteSpace(c))
                return TokenizerState.Valid;

            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '.' || c == '_')
                return TokenizerState.Valid;

            if (c != ')')
                return TokenizerState.Fail;


            for (int i = currentIndex + 1; i < fullExpression.Length;i++)
            {
                c = fullExpression[i];

                if (char.IsWhiteSpace(c))
                    continue;

                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9') || c == '(')
                {
                    _sawEnd = true;

                    return TokenizerState.Valid;
                }
                
                break;
            }

            return TokenizerState.Fail;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
    }
}