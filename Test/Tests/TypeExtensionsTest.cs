using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vici.Core.Test
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void IsNullable()
        {
            Assert.IsFalse(typeof(object).Inspector().IsNullable);
            Assert.IsFalse(typeof(int).Inspector().IsNullable);
            Assert.IsTrue(typeof(int?).Inspector().IsNullable);
            Assert.IsFalse(typeof(string).Inspector().IsNullable);
            Assert.IsFalse(typeof(DateTime).Inspector().IsNullable);
        }

        [TestMethod]
        public void CanBeNull()
        {
            Assert.IsTrue(typeof(object).Inspector().CanBeNull);
            Assert.IsFalse(typeof(int).Inspector().CanBeNull);
            Assert.IsTrue(typeof(int?).Inspector().CanBeNull);
            Assert.IsTrue(typeof(string).Inspector().CanBeNull);
            Assert.IsFalse(typeof(DateTime).Inspector().CanBeNull);
        }

        [TestMethod]
        public void DefaultValue()
        {
            Assert.IsNull(typeof(object).Inspector().DefaultValue());
            Assert.AreEqual(0, typeof(int).Inspector().DefaultValue());
            Assert.IsNull(typeof(int?).Inspector().DefaultValue());
            Assert.IsNull(typeof(string).Inspector().DefaultValue());
            Assert.AreEqual(new DateTime(), typeof(DateTime).Inspector().DefaultValue());
        }

        [TestMethod]
        public void GetRealType()
        {
            Assert.AreEqual(typeof(object), typeof(object).Inspector().RealType);
            Assert.AreEqual(typeof(int), typeof(int?).Inspector().RealType);
            Assert.AreEqual(typeof(string), typeof(string).Inspector().RealType);
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

        [TestMethod]
        public void GetAttribute()
        {
            Assert.IsInstanceOfType(typeof(TestClass1).Inspector().GetAttribute<Test1Attribute>(false), typeof(Test1Attribute));
            Assert.IsNull(typeof(TestClass2).Inspector().GetAttribute<Test1Attribute>(false));
        }
    }
}