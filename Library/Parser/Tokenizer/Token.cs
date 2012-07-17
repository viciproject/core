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
using System.Collections.Generic;

namespace Vici.Core.Parser
{
    public class Token
    {
        private ITokenMatcher _tokenMatcher;
        private string _token;
        private LinkedList<Token> _alternates;

        public TokenPosition TokenPosition;

        public Token()
        {
        }

        protected Token(ITokenMatcher tokenMatcher)
        {
            _tokenMatcher = tokenMatcher;
        }

        protected Token(ITokenMatcher tokenMatcher, string text)
        {
            _tokenMatcher = tokenMatcher;
            _token = text;
        }

        public ITokenMatcher TokenMatcher
        {
            get { return _tokenMatcher; }
            set { _tokenMatcher = value; }
        }

        public IEnumerable<Token> Alternates { get { return _alternates; } }

        public void AddAlternate(Token token)
        {
            if (_alternates == null)
                _alternates = new LinkedList<Token>();

            _alternates.AddLast(token);
        }

        public string Text
        {
            get { return _token; }
            set { _token = value; }
        }

        public override string ToString()
        {
            if (_tokenMatcher == null)
                return "(" + _token + ")";
            else
                return _tokenMatcher.GetType().Name + "(" + _token + ")";
        }
    }
}