using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vici.Core.Test
{
    [TestClass]
    public class StringConverterPluginTest
    {
        private class Custom1
        {
            public readonly string Content;

            public Custom1(string content)
            {
                Content = content;
            }
        }

        private class Custom2
        {
            public readonly string Content;

            public Custom2(string content)
            {
                Content = content;
            }
        }

        private class Custom3
        {
            public readonly string Content;

            public Custom3(string content)
            {
                Content = content;
            }
        }

        private class CustomStringConverter1 : IStringConverter
        {
            public bool TryConvert(string s, Type targetType, out object value)
            {
                value = null;

                if (targetType == typeof(Custom1))
                {
                    if (s == "NOCONVERT")
                        return false;

                    value = new Custom1(s);

                    return true;
                }

                return false;
            }
        }

        private class CustomStringConverter2 : IStringConverter<Custom2>
        {
            public bool TryConvert(string s, out Custom2 value)
            {
                value = null;

                if (s == "NOCONVERT")
                    return false;

                value = new Custom2(s);

                return true;
            }
        }

        [TestInitialize]
        public void Setup()
        {
            StringConverter.UnregisterAllStringConverters();

            StringConverter.RegisterStringConverter(new CustomStringConverter1());
            StringConverter.RegisterStringConverter(new CustomStringConverter2());
        }

        [TestMethod]
        public void TestCustomOk()
        {
            Assert.AreEqual("A", "A".To<Custom1>().Content);
        }

        [TestMethod]
        public void TestCustomFail()
        {
            Assert.IsNull("NOCONVERT".To<Custom1>());
        }

        [TestMethod]
        public void TestCustomTypedOk()
        {
            Assert.AreEqual("A", "A".To<Custom2>().Content);
        }

        [TestMethod]
        public void TestCustomTypedFail()
        {
            Assert.IsNull("NOCONVERT".To<Custom2>());
        }

        [TestMethod]
        public void TestCustomUnknown()
        {
            Assert.IsNull("A".To<Custom3>());
        }

    }
}