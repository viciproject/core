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
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using TestFixtureAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;

namespace Vici.Core.Test
{
    [TestClass]
    public class ObjectConverterTest
    {
        public enum TestEnum
        {
            One = 1,
            Two = 2
        }

        // ReSharper disable InconsistentNaming
        private int int0 = 0;
        private int int1 = 1;
        private int int2 = 2;
        private int int99 = 99;
        private int? intNull = null;
        private int? intN0 = 0;
        private int? intN1 = 1;
        private int? intN2 = 2;
        private int? intN99 = 99;

        private decimal dec0 = 0m;
        private decimal dec1 = 1m;
        private decimal dec2 = 2m;
        private decimal dec99 = 99m;
        private decimal? decNull = null;
        private decimal? decN0 = 0m;
        private decimal? decN1 = 1m;
        private decimal? decN2 = 2m;
        private decimal? decN99 = 99m;

        private string str0 = "0";
        private string str1 = "1";
        private string str2 = "2";
        private string str99 = "2";
        private string strNull = null;
        private string strEmpty = "";
        private string strOne = "One";
        private string strTwo = "Two";

        private char char0 = '0';
        private char char1 = '1';
        private char char2 = '2';
        private char charA = 'A';
        private char charB = 'B';
        private char? charNull = null;
        private char? charN1 = '1';
        private char? charN2 = '2';
        private char? charNA = 'A';
        private char? charNB = 'B';

        private bool boolTrue = true;
        private bool boolFalse = false;
        private bool? boolNull = null;
        private bool? boolNTrue = true;
        private bool? boolNFalse = false;

        private TestEnum enum1 = TestEnum.One;
        private TestEnum enum2 = TestEnum.Two;
        private TestEnum? enumNull = null;
        private TestEnum? enumN1 = TestEnum.One;
        private TestEnum? enumN2 = TestEnum.Two;

        private TestEnum enumDefault = default(TestEnum);
        private char charDefault = default(char);
        private bool boolDefault = default(bool);
        private decimal decDefault = default(decimal);

        // ReSharper restore InconsistentNaming

        [TestMethod]
        public void TestTypes()
        {
            Type[] types = new[] { typeof(int), typeof(string), typeof(bool), typeof(TestEnum), typeof(double) };

            object o = new object();

            foreach (Type t in types)
                Assert.IsInstanceOfType(o.Convert(t),t);

            foreach (Type t in types)
                Assert.AreEqual(t.Inspector().DefaultValue(), ObjectConverter.Convert(null,t));
        }

        [TestMethod]
        public void Test_Int_Null_Dynamic()
        {
            Assert.AreEqual(0, ObjectConverter.Convert(null, typeof(int)));
            Assert.IsNull(ObjectConverter.Convert(null, typeof(int?)));
        }

        [TestMethod]
        public void Test_Decimal_Null_Dynamic()
        {
            Assert.AreEqual(0m, ObjectConverter.Convert(null, typeof(decimal)));
            Assert.IsNull(ObjectConverter.Convert(null, typeof(decimal?)));
        }

        [TestMethod]
        public void Test_Char_Null_Dynamic()
        {
            Assert.AreEqual(charDefault, ObjectConverter.Convert(null, typeof(char)));
            Assert.IsNull(ObjectConverter.Convert(null, typeof(char?)));
        }

        [TestMethod]
        public void Test_Bool_Null_Dynamic()
        {
            Assert.AreEqual(boolDefault, ObjectConverter.Convert(null, typeof(bool)));
            Assert.IsNull(ObjectConverter.Convert(null, typeof(bool?)));
        }

        [TestMethod]
        public void Test_String_Null_Dynamic()
        {
            Assert.IsNull(ObjectConverter.Convert(null, typeof(string)));
        }

        [TestMethod]
        public void Test_Int_Null_Generic()
        {
            Assert.AreEqual(0, ObjectConverter.Convert<int>(null));
            Assert.IsNull(ObjectConverter.Convert<int?>(null));
        }

        [TestMethod]
        public void Test_Decimal_Null_Generic()
        {
            Assert.AreEqual(0m, ObjectConverter.Convert<decimal>(null));
            Assert.IsNull(ObjectConverter.Convert<decimal?>(null));
        }

        [TestMethod]
        public void Test_Char_Null_Generic()
        {
            Assert.AreEqual(charDefault, ObjectConverter.Convert<char>(null));
            Assert.IsNull(ObjectConverter.Convert<char?>(null));
        }

        [TestMethod]
        public void Test_String_Null_Generic()
        {
            Assert.IsNull(ObjectConverter.Convert<string>(null));
        }

        [TestMethod]
        public void Test_Int_Enum_Dynamic()
        {
            Assert.AreEqual(TestEnum.One, int1.Convert(typeof(TestEnum)));
            Assert.AreEqual(TestEnum.One, intN1.Convert(typeof(TestEnum)));
            Assert.AreEqual(1, enum1.Convert(typeof(int)));
            Assert.AreEqual(1, enumN1.Convert(typeof(int)));

            Assert.AreEqual(TestEnum.Two, int2.Convert(typeof(TestEnum)));
            Assert.AreEqual(TestEnum.Two, intN2.Convert(typeof(TestEnum)));
            Assert.AreEqual(2, enum2.Convert(typeof(int)));
            Assert.AreEqual(2, enumN2.Convert(typeof(int)));

            Assert.AreEqual(enumDefault, int99.Convert(typeof(TestEnum)));
            Assert.AreEqual(enumDefault, intN99.Convert(typeof(TestEnum)));

            Assert.AreEqual(TestEnum.One, int1.Convert(typeof(TestEnum?)));
            Assert.AreEqual(1, enum1.Convert(typeof(int?)));

            Assert.AreEqual(TestEnum.Two, int2.Convert(typeof(TestEnum?)));
            Assert.AreEqual(2, enum2.Convert(typeof(int?)));

            Assert.AreEqual(null, int99.Convert(typeof(TestEnum?)));
        }

        [TestMethod]
        public void Test_Decimal_Enum_Dynamic()
        {
            Assert.AreEqual(TestEnum.One, dec1.Convert(typeof(TestEnum)));
            Assert.AreEqual(TestEnum.One, decN1.Convert(typeof(TestEnum)));
            Assert.AreEqual(1m, enum1.Convert(typeof(decimal)));
            Assert.AreEqual(1m, enumN1.Convert(typeof(decimal)));

            Assert.AreEqual(TestEnum.Two, dec2.Convert(typeof(TestEnum)));
            Assert.AreEqual(TestEnum.Two, decN2.Convert(typeof(TestEnum)));
            Assert.AreEqual(2m, enum2.Convert(typeof(decimal)));
            Assert.AreEqual(2m, enumN2.Convert(typeof(decimal)));

            Assert.AreEqual(enumDefault, dec99.Convert(typeof(TestEnum)));
            Assert.AreEqual(enumDefault, decN99.Convert(typeof(TestEnum)));

            Assert.AreEqual(TestEnum.One, dec1.Convert(typeof(TestEnum?)));
            Assert.AreEqual(1m, enum1.Convert(typeof(decimal?)));

            Assert.AreEqual(TestEnum.Two, dec2.Convert(typeof(TestEnum?)));
            Assert.AreEqual(2m, enum2.Convert(typeof(decimal?)));

            Assert.AreEqual(null, dec99.Convert(typeof(TestEnum?)));
        }

        [TestMethod]
        public void Test_Int_Bool_Dynamic()
        {
            Assert.AreEqual(true, int1.Convert(typeof(bool)));
            Assert.AreEqual(true, intN1.Convert(typeof(bool)));
            Assert.AreEqual(1, boolTrue.Convert(typeof(int)));
            Assert.AreEqual(1, boolNTrue.Convert(typeof(int)));

            Assert.AreEqual(false, int0.Convert(typeof(bool)));
            Assert.AreEqual(false, intN0.Convert(typeof(bool)));
            Assert.AreEqual(int0, boolFalse.Convert(typeof(int)));
            Assert.AreEqual(int0, boolNFalse.Convert(typeof(int)));

            Assert.AreEqual(true, int99.Convert(typeof(bool)));
            Assert.AreEqual(true, intN99.Convert(typeof(bool)));

            Assert.AreEqual(true, int1.Convert(typeof(bool?)));
            Assert.AreEqual(true, intN1.Convert(typeof(bool?)));
            Assert.AreEqual(1, boolTrue.Convert(typeof(int)));
            Assert.AreEqual(1, boolNTrue.Convert(typeof(int)));

            Assert.AreEqual(false, int0.Convert(typeof(bool?)));
            Assert.AreEqual(false, intN0.Convert(typeof(bool?)));
            Assert.AreEqual(int0, boolFalse.Convert(typeof(int?)));
            Assert.AreEqual(int0, boolNFalse.Convert(typeof(int?)));

            Assert.AreEqual(true, int99.Convert(typeof(bool?)));
            Assert.AreEqual(true, intN99.Convert(typeof(bool?)));
        }

        [TestMethod]
        public void Test_Char_String_Dynamic()
        {
            Assert.AreEqual("A", ObjectConverter.Convert(charA,typeof(string)));
            Assert.AreEqual(charA,ObjectConverter.Convert("A",typeof(char)));

            Assert.AreEqual("A", ObjectConverter.Convert(charNA, typeof(string)));
            Assert.AreEqual(charA, ObjectConverter.Convert("A", typeof(char)));
        }

        private enum EnumType
        {
            Zero=0,One=1
        }

        [TestMethod]
        public void MiscTest()
        {
            int intValue = 5;
            int? nIntValue = 6;
            EnumType enumZero = EnumType.Zero;
            EnumType? enumOne = EnumType.One;
            short shortValue = 7;
            short? nShortValue = 8;
            decimal decValue = 9;
            decimal? nDecValue = 10;

            Assert.AreEqual(5, intValue.Convert(typeof(int)));
            Assert.AreEqual(6, nIntValue.Convert(typeof(int)));
            Assert.AreEqual(5, intValue.Convert(typeof(int?)));
            Assert.AreEqual(6, nIntValue.Convert(typeof(int?)));
            Assert.AreEqual(7, shortValue.Convert(typeof(int?)));
            Assert.AreEqual(8, nShortValue.Convert(typeof(int?)));
            Assert.AreEqual((short)5, intValue.Convert(typeof(short)));
            Assert.AreEqual((short)6, nIntValue.Convert(typeof(short)));
            Assert.AreEqual((short)5, intValue.Convert(typeof(short?)));
            Assert.AreEqual((short)6, nIntValue.Convert(typeof(short?)));
            Assert.AreEqual(0, enumZero.Convert(typeof(int)));
            Assert.AreEqual(1, enumOne.Convert(typeof(int)));
            Assert.AreEqual((short)0, enumZero.Convert(typeof(short)));
            Assert.AreEqual((short)1, enumOne.Convert(typeof(short)));

            Assert.AreEqual(9, decValue.Convert<int>());
            Assert.AreEqual(10, nDecValue.Convert<int>());

            Assert.AreEqual(EnumType.One, ObjectConverter.Convert<EnumType>(1));
            Assert.AreEqual(EnumType.One, ObjectConverter.Convert((short)1, typeof(EnumType)));
            Assert.AreEqual(EnumType.One, ObjectConverter.Convert((byte)1, typeof(EnumType)));
            Assert.AreEqual(EnumType.One, ObjectConverter.Convert((long)1, typeof(EnumType)));
            Assert.AreEqual(EnumType.Zero, ObjectConverter.Convert(0, typeof(EnumType?)));
            Assert.AreEqual(EnumType.Zero, ObjectConverter.Convert(EnumType.Zero, typeof(EnumType?)));

            Assert.IsNull(ObjectConverter.Convert(null, typeof(int?)));
            Assert.IsNull(ObjectConverter.Convert(null, typeof(EnumType?)));

            Guid guid = ObjectConverter.Convert<Guid>(new byte[16]);

            Assert.AreEqual(guid, new Guid(new byte[16]));
        }
        


    }
}