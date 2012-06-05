using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Vici.Core.Test
{
    [TestClass]
    public class StringConverterTest
    {
        public enum TestEnum
        {
            One = 1,
            Two = 2,
            Ten = 10
        }

        public enum EmptyEnum
        {
        }

        [TestMethod]
        public void TestTypes()
        {
            Type[] types = new [] { typeof(int), typeof(string), typeof(bool), typeof(TestEnum), typeof(double), typeof(EmptyEnum) };

            foreach (Type t in types)
                Assert.IsInstanceOfType("123".To(t), t);
        }

        [TestMethod]
        public void ToInt()
        {
            Assert.AreEqual(123,"123".To<int>());
            Assert.AreEqual(0, "0".To<int>());

            Assert.AreEqual(123, "123".To(typeof(int)));
            Assert.AreEqual(0, "0".To(typeof(int)));
        }

        [TestMethod]
        public void ToIntFail()
        {
            Assert.AreEqual(0, "123A".To<int>());
        }

        [TestMethod]
        public void ToNullableInt()
        {
            Assert.AreEqual(123, "123".To<int?>());
        }

        [TestMethod]
        public void ToNullableIntFail()
        {
            Assert.IsNull("123A".To<int?>());
        }

        [TestMethod]
        public void ToIntEmpty()
        {
            Assert.AreEqual(0, "".To<int>());
        }

        [TestMethod]
        public void ToNullableIntEmpty()
        {
            Assert.IsNull("".To<int?>());
        }

        [TestMethod]
        public void ToIntNull()
        {
            Assert.AreEqual(0, ((string)null).To<int>());
        }

        [TestMethod]
        public void ToNullableIntNull()
        {
            Assert.IsNull(((string)null).To<int?>());
        }

        [TestMethod]
        public void ToEnum()
        {
            Assert.AreEqual(TestEnum.One, "1".To<TestEnum>());
            Assert.AreEqual(TestEnum.Two, "2".To<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, "10".To<TestEnum>());
        }

        [TestMethod]
        public void ToEnumNamed()
        {
            Assert.AreEqual(TestEnum.One, "One".To<TestEnum>());
            Assert.AreEqual(TestEnum.Two, "Two".To<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, "Ten".To<TestEnum>());
        }

        [TestMethod]
        public void ToEnumFail()
        {
            Assert.AreEqual((TestEnum)0, "123".To<TestEnum>());
            Assert.AreEqual((TestEnum)0, "123".To(typeof(TestEnum)));
        }

        [TestMethod]
        public void ToEnumNamedFail()
        {
            Assert.AreEqual((TestEnum) 0, "Three".To<TestEnum>());
        }

        [TestMethod]
        public void ToNullableEnum()
        {
            Assert.AreEqual(TestEnum.One, "1".To<TestEnum?>());
            Assert.AreEqual(TestEnum.Two, "2".To<TestEnum?>());
            Assert.AreEqual(TestEnum.Ten, "10".To<TestEnum?>());
        }

        [TestMethod]
        public void ToNullableEnumFail()
        {
            Assert.IsNull("123".To<TestEnum?>());
        }

        [TestMethod]
        public void ToEnumEmpty()
        {
            Assert.AreEqual((TestEnum)0, "".To<TestEnum>());
        }

        [TestMethod]
        public void ToNullableEnumEmpty()
        {
            Assert.IsNull("".To<TestEnum?>());
        }

        [TestMethod]
        public void ToEnumNull()
        {
            Assert.AreEqual((TestEnum)0, ((string)null).To<TestEnum>());
        }

        [TestMethod]
        public void ToNullableEnumNull()
        {
            Assert.IsNull(((string)null).To<TestEnum?>());
        }

    }
}