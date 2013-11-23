using System;
using NUnit.Framework;

namespace Vici.Core.Test
{
    [TestFixture]
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

        [Test]
        public void TestTypes()
        {
            Type[] types = new [] { typeof(int), typeof(string), typeof(bool), typeof(TestEnum), typeof(double), typeof(EmptyEnum) };

            foreach (Type t in types)
                Assert.IsInstanceOf(t,"123".To(t));
        }

        [Test]
        public void ToInt()
        {
            Assert.AreEqual(123,"123".To<int>());
            Assert.AreEqual(0, "0".To<int>());

            Assert.AreEqual(123, "123".To(typeof(int)));
            Assert.AreEqual(0, "0".To(typeof(int)));
        }

        [Test]
        public void ToIntFail()
        {
            Assert.AreEqual(0, "123A".To<int>());
        }

        [Test]
        public void ToNullableInt()
        {
            Assert.AreEqual(123, "123".To<int?>());
        }

        [Test]
        public void ToNullableIntFail()
        {
            Assert.IsNull("123A".To<int?>());
        }

        [Test]
        public void ToIntEmpty()
        {
            Assert.AreEqual(0, "".To<int>());
        }

        [Test]
        public void ToNullableIntEmpty()
        {
            Assert.IsNull("".To<int?>());
        }

        [Test]
        public void ToIntNull()
        {
            Assert.AreEqual(0, ((string)null).To<int>());
        }

        [Test]
        public void ToNullableIntNull()
        {
            Assert.IsNull(((string)null).To<int?>());
        }

        [Test]
        public void ToEnum()
        {
            Assert.AreEqual(TestEnum.One, "1".To<TestEnum>());
            Assert.AreEqual(TestEnum.Two, "2".To<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, "10".To<TestEnum>());
        }

        [Test]
        public void ToEnumNamed()
        {
            Assert.AreEqual(TestEnum.One, "One".To<TestEnum>());
            Assert.AreEqual(TestEnum.Two, "Two".To<TestEnum>());
            Assert.AreEqual(TestEnum.Ten, "Ten".To<TestEnum>());
        }

        [Test]
        public void ToEnumFail()
        {
            Assert.AreEqual((TestEnum)0, "123".To<TestEnum>());
            Assert.AreEqual((TestEnum)0, "123".To(typeof(TestEnum)));
        }

        [Test]
        public void ToEnumNamedFail()
        {
            Assert.AreEqual((TestEnum) 0, "Three".To<TestEnum>());
        }

        [Test]
        public void ToNullableEnum()
        {
            Assert.AreEqual(TestEnum.One, "1".To<TestEnum?>());
            Assert.AreEqual(TestEnum.Two, "2".To<TestEnum?>());
            Assert.AreEqual(TestEnum.Ten, "10".To<TestEnum?>());
        }

        [Test]
        public void ToNullableEnumFail()
        {
            Assert.IsNull("123".To<TestEnum?>());
        }

        [Test]
        public void ToEnumEmpty()
        {
            Assert.AreEqual((TestEnum)0, "".To<TestEnum>());
        }

        [Test]
        public void ToNullableEnumEmpty()
        {
            Assert.IsNull("".To<TestEnum?>());
        }

        [Test]
        public void ToEnumNull()
        {
            Assert.AreEqual((TestEnum)0, ((string)null).To<TestEnum>());
        }

        [Test]
        public void ToNullableEnumNull()
        {
            Assert.IsNull(((string)null).To<TestEnum?>());
        }

    }
}