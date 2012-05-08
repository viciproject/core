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
    public struct TokenPosition
    {
        private int _line;
        private int _column;

        public static TokenPosition Unknown = new TokenPosition(1, 1);

        public TokenPosition(int lineNum, int column)
        {
            _line = lineNum;
            _column = column;
        }

        public TokenPosition(TokenPosition parent, TokenPosition current)
        {
            _line = parent.Line + current.Line - 1;

            _column = current.Column;

            if (current.Line <= 1)
                _column = parent.Column + current.Column - 1;
        }

        public void ChangeBase(TokenPosition basePosition)
        {
            if (_line <= 1)
                _column = basePosition.Column + _column - 1;

            _line = basePosition.Line + _line - 1;
        }

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public static TokenPosition[] GetPositionsFromString(string s)
        {
            return GetPositionsFromString(s, null);
        }

        public static TokenPosition[] GetPositionsFromString(string s, TokenPosition? basePosition)
        {
            TokenPosition[] positions = new TokenPosition[s.Length];

            int currentLine = 1;
            int currentColumn = 1;

            for (int textIndex = 0; textIndex < s.Length; textIndex++)
            {
                char c = s[textIndex];

                positions[textIndex].Line = currentLine;
                positions[textIndex].Column = currentColumn;

                if (basePosition != null)
                    positions[textIndex].ChangeBase(basePosition.Value);

                if (c == '\n')
                {
                    currentLine++;
                    currentColumn = 1;
                }
                else if (c != '\r')
                    currentColumn++;
            }

            return positions;
        }

        public override string ToString()
        {
            return "(L " + Line + " , C " + Column + ")";
        }
    }
}