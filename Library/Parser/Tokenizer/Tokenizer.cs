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
using System.Collections.Generic;

namespace Vici.Core.Parser
{
    public class Tokenizer : Tokenizer<Token>
    {
        
    }

    public class Tokenizer<T> where T:Token, new()
    {
        readonly List<ITokenMatcher> _tokenMatchers = new List<ITokenMatcher>();

        private readonly bool _allowFillerTokens;

        public Tokenizer()
        {
        }

        public Tokenizer(bool allowFillerTokens)
        {
            _allowFillerTokens = allowFillerTokens;
        }

        public void AddTokenMatcher(ITokenMatcher tokenMatcher)
        {
            _tokenMatchers.Add(tokenMatcher);
        }

        private class SuccessfulMatch
        {
            public string Token;

            public int StartIndex;
            public int Length;

            public List<TokenMatcher> Matches;
        }

        public T[] Tokenize(string s)
        {
            return Tokenize(s, TokenPosition.Unknown);
        }

        public T[] Tokenize(string s, TokenPosition position)
        {
            List<T> tokens = new List<T>();

            TokenMatcher[] tokenMatchers = new TokenMatcher[_tokenMatchers.Count];

            for(int i=0;i<tokenMatchers.Length;i++)
                tokenMatchers[i] = new TokenMatcher(_tokenMatchers[i]);

            List<TokenMatcher> successfulTokens = new List<TokenMatcher>(5);
            SuccessfulMatch successMatch = null;
            
            Reset(tokenMatchers);

            TokenPosition[] positions = TokenPosition.GetPositionsFromString(s , position);

            int firstValidIndex = -1;
            int lastSavedIndex = -1;

            string filler = "";
            TokenPosition fillerPosition = TokenPosition.Unknown;

            for (int textIndex = 0; textIndex < s.Length; textIndex++)
            {
                char c = s[textIndex];

                bool foundToken = false;
                successfulTokens.Clear();

                //TODO: parralel processing in .NET 4.0
                foreach (TokenMatcher tokenMatcher in tokenMatchers)
                {
                    TokenizerState state = tokenMatcher.Feed(c, s, textIndex);

                    if (state == TokenizerState.Valid)
                        foundToken = true;
                    else if (state == TokenizerState.Success)
                        successfulTokens.Add(tokenMatcher);
                }

                if (successfulTokens.Count > 0)
                {
                    successMatch = new SuccessfulMatch();

                    successMatch.StartIndex = firstValidIndex;
                    successMatch.Length = textIndex - firstValidIndex;

                    successMatch.Token = successfulTokens[0].TranslateToken(s.Substring(firstValidIndex, textIndex - firstValidIndex));

                    successMatch.Matches = new List<TokenMatcher>(successfulTokens);
                }

                if (foundToken)
                {
                    if (firstValidIndex < 0)
                        firstValidIndex = textIndex;

                    continue;
                }

                if (successMatch == null)
                {
                    if (_allowFillerTokens)
                    {
                        filler += s[++lastSavedIndex];
                        
                        textIndex = lastSavedIndex;

                        fillerPosition = positions[lastSavedIndex];

                        Reset(tokenMatchers);

                        firstValidIndex = -1;

                        continue;
                    }
                    else
                    {
                        string badToken;

                        if (firstValidIndex < 0)
                            badToken = s.Substring(lastSavedIndex + 1, 1);
                        else
                            badToken = s.Substring(lastSavedIndex + 1, firstValidIndex - lastSavedIndex);

                        throw new UnknownTokenException(positions[lastSavedIndex + 1], badToken);
                    }
                }

                if (filler.Length > 0)
                {
                    T fillerToken = CreateToken(null,filler);
                    
                    fillerToken.TokenPosition = fillerPosition;

                    tokens.Add(fillerToken);

                    filler = "";
                }

                T token = CreateToken(successMatch.Matches[0].Matcher, successMatch.Token);

                for (int i = 1; i < successMatch.Matches.Count; i++)
                {
                    T alternateToken = CreateToken(successMatch.Matches[i].Matcher, successMatch.Token);

                    alternateToken.TokenPosition = positions[firstValidIndex];

                    token.AddAlternate(alternateToken);
                }

                token.TokenPosition = positions[firstValidIndex];
                tokens.Add(token);

                lastSavedIndex = textIndex - 1;

                textIndex = successMatch.StartIndex + successMatch.Length-1;

                firstValidIndex = -1;
                successMatch = null;

                Reset(tokenMatchers);
            }

            successfulTokens.Clear();

            foreach (TokenMatcher tokenMatcher in tokenMatchers)
            {
                if (tokenMatcher.Feed('\0', s, s.Length) == TokenizerState.Success)
                    successfulTokens.Add(tokenMatcher);
            }

            if (_allowFillerTokens && filler.Length > 0)
            {
                T fillerToken = CreateToken(null, filler);

                fillerToken.TokenPosition = fillerPosition;

                tokens.Add(fillerToken);
            }

            if (successfulTokens.Count > 0)
            {
                string tokenText = s.Substring(firstValidIndex, s.Length - firstValidIndex);

                tokenText = successfulTokens[0].TranslateToken(tokenText);

                T token = CreateToken(successfulTokens[0].Matcher, tokenText);

                for (int i = 1; i < successfulTokens.Count; i++)
                    token.AddAlternate(CreateToken(successfulTokens[i].Matcher, tokenText));

                tokens.Add(token);
            }

            return tokens.ToArray();
        }

        private void Reset(TokenMatcher[] matchers)
        {
            foreach (TokenMatcher td in matchers)
                td.Reset();
        }

        public virtual T CreateToken(ITokenMatcher tokenMatcher, string token)
        {
            T t = new T();

            t.Text = token;
            t.TokenMatcher = tokenMatcher;

            return t;
        }

    }
}
