using NUnit.Framework;

namespace Vici.Core.Test
{
    [TestFixture]
    public class SafeDictionaryTest
    {
        [Test]
        public void TestDefaultDefaultString()
        {
            var dic = new SafeDictionary<string, string>();

            dic["A"] = "AA";
            dic["B"] = "BB";

            Assert.AreEqual("AA",dic["A"]);
            Assert.IsNull(dic["C"]);
        }

        [Test]
        public void TestCustomDefaultString()
        {
            var dic = new SafeDictionary<string, string>();

            dic.DefaultValue = "";

            dic["A"] = "AA";
            dic["B"] = "BB";

            Assert.AreEqual("AA", dic["A"]);
            Assert.AreEqual("", dic["C"]);
        }

        [Test]
        public void TestDefaultDefaultInt()
        {
            var dic = new SafeDictionary<string, int>();

            dic["A"] = 1;
            dic["B"] = 2;

            Assert.AreEqual(1, dic["A"]);
            Assert.AreEqual(0, dic["C"]);
        }

        [Test]
        public void TestCustomDefaultInt()
        {
            var dic = new SafeDictionary<string, int>();

            dic.DefaultValue = 999;

            dic["A"] = 1;
            dic["B"] = 2;

            Assert.AreEqual(1, dic["A"]);
            Assert.AreEqual(999, dic["C"]);
        }

        [Test]
        public void TestSafeStringDictionaryCaseSensitive()
        {
            var dic = new SafeStringDictionary<string>();

            dic["A"] = "AA";
            dic["B"] = "BB";

            Assert.AreEqual("AA", dic["A"]);
            Assert.IsNull(dic["a"]);
            Assert.IsNull(dic["C"]);
        }

        [Test]
        public void TestSafeStringDictionaryCaseInsensitive()
        {
            var dic = new SafeStringDictionary<string>(true);

            dic["A"] = "AA";
            dic["B"] = "BB";

            Assert.AreEqual("AA", dic["A"]);
            Assert.AreEqual("AA", dic["a"]);
            Assert.IsNull(dic["C"]);
        }
    }
}