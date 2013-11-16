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
using System.Runtime.Serialization;

namespace Vici.Core.Parser
{
    public class TemplateParserException : ParserException, IPositionedException
    {
        private readonly TokenPosition _tokenPosition;
        
        public TemplateParserException(string message, TokenPosition tokenPosition) : base(message)
        {
            _tokenPosition = tokenPosition;
        }

        public TemplateParserException(string message, Exception innerException, TokenPosition tokenPosition) : base(message, innerException)
        {
            _tokenPosition = tokenPosition;
        }

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        public TemplateParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif

        public TokenPosition Position
        {
            get { return _tokenPosition; }
        }
    }
}