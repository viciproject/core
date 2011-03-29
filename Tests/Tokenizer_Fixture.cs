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
using NUnit.Framework;
using Vici.Core.Parser;

namespace Vici.Core.Test
{
    [TestFixture]
    public class Tokenizer_Fixture
    {
        [Test]
        public void TestStringLiteral()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new StringLiteralMatcher());
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());
            tokenizer.AddTokenMatcher(new CharMatcher('+'));

            Token[] tokens = tokenizer.Tokenize("\"test1\" + \"test2\"");

            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("\"test1\"", tokens[0].Text);
            Assert.AreEqual("\"test2\"", tokens[4].Text);

            tokens = tokenizer.Tokenize("\"test1\" + \"test\\\"2\"");

            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("\"test1\"", tokens[0].Text);
            Assert.AreEqual("\"test\\\"2\"", tokens[4].Text);
        }

        [Test]
        public void NumericLiterals()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new IntegerLiteralMatcher());
            tokenizer.AddTokenMatcher(new DecimalLiteralMatcher());
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());

            Token[] tokens;

            tokens = tokenizer.Tokenize("10 10.0");

            Assert.AreEqual(3,tokens.Length);
            Assert.AreEqual("10",tokens[0].Text);
            Assert.AreEqual("10.0",tokens[2].Text);

            tokens = tokenizer.Tokenize("10m 10ul");

            Assert.AreEqual(3, tokens.Length);
            Assert.AreEqual("10m", tokens[0].Text);
            Assert.AreEqual("10ul", tokens[2].Text);

            tokens = tokenizer.Tokenize("10f 10l");

            Assert.AreEqual(3, tokens.Length);
            Assert.AreEqual("10f", tokens[0].Text);
            Assert.AreEqual("10l", tokens[2].Text);
        }

        [Test]
        public void TestFallback()
        {
            ITokenMatcher matcher1 = new CharMatcher('(');
            ITokenMatcher matcher2 = new CharMatcher(')');
            ITokenMatcher matcher3 = new StringMatcher("(test)");
            ITokenMatcher matcher4 = new AnyCharMatcher("abcdefghijklmnopqrstuvwxyz");

            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(matcher1);
            tokenizer.AddTokenMatcher(matcher2);
            tokenizer.AddTokenMatcher(matcher3);
            tokenizer.AddTokenMatcher(matcher4);

            Token[] tokens = tokenizer.Tokenize("(test)(x)");

            Assert.AreEqual(4,tokens.Length);

        }

        [Test]
        public void TestAnySequence()
        {
            ITokenMatcher matcher = new CompositeMatcher(
                new AnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_"),
                new SequenceOfAnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789")
                );

            ITokenMatcher alphaMatcher = new SequenceOfAnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");

            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(matcher);
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());
            tokenizer.AddTokenMatcher(new CharMatcher('.'));

            Token[] tokens = tokenizer.Tokenize("Math.Max");

            Assert.AreEqual(3,tokens.Length);
            Assert.AreEqual("Math",tokens[0].Text);
            Assert.AreEqual(".", tokens[1].Text);
            Assert.AreEqual("Max", tokens[2].Text);




        }

        [Test]
        [ExpectedException(typeof(UnknownTokenException))]
        public void BadToken()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new IntegerLiteralMatcher());
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());

            tokenizer.Tokenize("5 A");
        }

        [Test]
        public void BadTokenPosition()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new IntegerLiteralMatcher());
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());

            try
            {
                tokenizer.Tokenize("5 A");
            }
            catch (UnknownTokenException ex)
            {
                Assert.AreEqual(3, ex.Position.Column);
                Assert.AreEqual(1, ex.Position.Line);
                Assert.AreEqual("A", ex.Token);
            }

            try
            {
                tokenizer.Tokenize("5 4\r\n2\r\n   X\r\n5");
            }
            catch (UnknownTokenException ex)
            {
                Assert.AreEqual(4, ex.Position.Column);
                Assert.AreEqual(3, ex.Position.Line);
                Assert.AreEqual("X",ex.Token);
            }

        }


        [Test]
        public void StartsAndEndsWithToken()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new StartsAndEndsWithMatcher("<!--","-->",'"'));
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());

            Token[] tokens;

            tokens = tokenizer.Tokenize("<!--test-->  <!-- test 2 -->");

            Assert.AreEqual(3,tokens.Length);

            tokens = tokenizer.Tokenize("<!--test \"-->\"-->  <!-- test 2 -->");

            Assert.AreEqual(3, tokens.Length);
        }
    }
}