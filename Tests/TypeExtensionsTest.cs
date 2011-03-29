using System;
using System.Linq;
using NUnit.Framework;

namespace Vici.Core.Test
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void IsNullable()
        {
            Assert.IsFalse(typeof(object).IsNullable());
            Assert.IsFalse(typeof(int).IsNullable());
            Assert.IsTrue(typeof(int?).IsNullable());
            Assert.IsFalse(typeof(string).IsNullable());
            Assert.IsFalse(typeof(DateTime).IsNullable());
        }

        [Test]
        public void CanBeNull()
        {
            Assert.IsTrue(typeof(object).CanBeNull());
            Assert.IsFalse(typeof(int).CanBeNull());
            Assert.IsTrue(typeof(int?).CanBeNull());
            Assert.IsTrue(typeof(string).CanBeNull());
            Assert.IsFalse(typeof(DateTime).CanBeNull());
        }

        [Test]
        public void DefaultValue()
        {
            Assert.IsNull(typeof(object).DefaultValue());
            Assert.AreEqual(0,typeof(int).DefaultValue());
            Assert.IsNull(typeof(int?).DefaultValue());
            Assert.IsNull(typeof(string).DefaultValue());
            Assert.AreEqual(new DateTime(),typeof(DateTime).DefaultValue());
        }

        [Test]
        public void GetRealType()
        {
            Assert.AreEqual(typeof(object),typeof(object).GetRealType());
            Assert.AreEqual(typeof(int), typeof(int?).GetRealType());
            Assert.AreEqual(typeof(string), typeof(string).GetRealType());
        }

        private class Test1Attribute  : Attribute
        {
            
        }

        [Test1]
        private class TestClass1
        {
            
        }

        private class TestClass2
        {

        }

        [Test]
        public void GetAttribute()
        {
            Assert.IsInstanceOfType(typeof(Test1Attribute),typeof(TestClass1).GetAttribute<Test1Attribute>(false));
            Assert.IsNull(typeof(TestClass2).GetAttribute<Test1Attribute>(false));
        }
    }
}