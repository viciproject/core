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

using System.Linq;
using NUnit.Framework;
using Vici.Core.BinaryExtensions;

namespace Vici.Core.Test
{
    [TestFixture]
    public class BinaryExtensionsTest
    {
        [Test]
        public void ToHex()
        {
            byte[] bytes = new byte[] {0x12, 0xAB, 0xF1};

            Assert.AreEqual("12abf1", bytes.ToHex());
            Assert.AreEqual("12abf1", bytes.ToHex(false));
            Assert.AreEqual("12ABF1", bytes.ToHex(true));
        }

        [Test]
        public void BinarySerializerTest()
        {
            string x = "ABC";

            byte[] bytes = BinarySerializer.Serialize(x);

            Assert.AreEqual("ABC",BinarySerializer.Deserialize<string>(bytes));
        }
    }
}
