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
    public class IntegerLiteralMatcher : ITokenMatcher, ITokenProcessor
    {
        private enum Stage
        {
            Num1,
            Num2,
            Suffix,
            Done
        }

        private const string SUFFIXES = "FfDdMmUuLl";

        private Stage _stage;
        private char _suffix;

        public ITokenProcessor CreateTokenProcessor()
        {
            return new IntegerLiteralMatcher();
        }

        public void ResetState()
        {
            _stage = Stage.Num1;
        }

        public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
        {
            bool isDigit = (c >= '0' && c <= '9');

            switch (_stage)
            {
                case Stage.Num1:
                    {
                        if (!isDigit)
                            return TokenizerState.Fail;

                        _stage = Stage.Num2;
                    }
                    break;

                case Stage.Num2:
                    {
                        if (SUFFIXES.IndexOf(c) >= 0)
                        {
                            _stage = Stage.Suffix;
                            _suffix = c;
                        }
                        else if (!isDigit)
                        {
                            return TokenizerState.Success;
                        }
                    }
                    break;

                case Stage.Suffix:
                    {
                        if ((_suffix == 'l' || _suffix == 'L') && (c == 'U' || c == 'u'))
                        {
                            _stage = Stage.Done;
                        }
                        else if ((_suffix == 'u' || _suffix == 'U') && (c == 'L' || c == 'l'))
                        {
                            _stage = Stage.Done;
                        }
                        else
                        {
                            return TokenizerState.Success;
                        }
                    }
                    break;

                case Stage.Done:
                    return TokenizerState.Success;
            }

            return TokenizerState.Valid;
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }
        
    }
}