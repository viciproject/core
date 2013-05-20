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
using System.Text.RegularExpressions;

namespace Vici.Core.Parser
{
    public class TemplateToken : Token
    {
        private static readonly object _lock = new object();
        private ParameterizedExpression _parameterizedExpression;

        public TemplateToken()
        {
            throw new NotSupportedException();
        }

        public TemplateToken(TemplateTokenMatcher tokenMatcher, string token) : base(tokenMatcher, token)
        {
        }

        private TemplateTokenMatcher Matcher
        {
            get { return (TemplateTokenMatcher) TokenMatcher; }
        }

        public bool RemoveEmptyLine
    	{
            get { return Matcher != null && Matcher.RemoveEmptyLine; }
    	}

    	public TemplateTokenType TokenType
    	{
    		get { return Matcher.TokenType; }
    	}

        public string TokenId
        {
            get { return Matcher.TokenId; }
        }

        public ParameterizedExpression ExtractParameters()
        {
            lock (_lock)
            {
                if (_parameterizedExpression == null)
                    _parameterizedExpression = new ParameterizedExpression(Text);
            }

            return _parameterizedExpression;
        }

        public class ParameterizedExpression
        {
            public readonly string MainExpression;
            public readonly Dictionary<string, string> Parameters;

            public ParameterizedExpression(string expression)
            {
                Parameters = new Dictionary<string, string>();

                MatchCollection matches = Regex.Matches(expression, @"[,\s]*@(?<varname>[a-zA-Z_$][a-zA-Z_$0-9]*)\s*=");

                int index = -1;
                string varName = null;

                if (matches.Count < 1)
                {
                    MainExpression = expression;
                    return;
                }

                foreach (Match match in matches)
                {
                    if (index == -1)
                    {
                        MainExpression = expression.Substring(0, match.Index).Trim();
                    }
                    else
                    {
                        if (varName != null)
                            Parameters[varName] = expression.Substring(index, match.Index - index).Trim();
                    }

                    varName = match.Groups["varname"].Value;

                    index = match.Index + match.Length;
                }

                if (varName != null)
                    Parameters[varName] = expression.Substring(index, expression.Length - index).Trim();
            }
        }

    }
}