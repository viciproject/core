using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vici.Core.Json;

namespace Vici.Core.Test
{
    [TestFixture]
    public class JsonSeralizerTestFixture
    {
        [Test]
        public void TestArray()
        {
            string json = JsonSerializer.ToJson(new[] { 1, 2, 3 });

            Assert.AreEqual("[1,2,3]", json);
        }

        [Test]
        public void TestList()
        {
            string json = JsonSerializer.ToJson(new List<int>(new[] { 1, 2, 3 }));

            Assert.AreEqual("[1,2,3]", json);
        }

        [Test]
        public void TestInt()
        {
            Assert.AreEqual("123", JsonSerializer.ToJson(123));
        }

        [Test]
        public void TestSring()
        {
            Assert.AreEqual("\"abc\"", JsonSerializer.ToJson("abc"));
        }

        [Test]
        public void TestDouble()
        {
            Assert.AreEqual("12.34", JsonSerializer.ToJson(12.34));
        }

        [Test]
        public void TestBool()
        {
            Assert.AreEqual("true", JsonSerializer.ToJson(true));
            Assert.AreEqual("false", JsonSerializer.ToJson(false));
        }

        [Test]
        public void TestDictionary()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            dic["a"] = 1;
            dic["b"] = 2;

            Assert.AreEqual("{\"a\":1,\"b\":2}", JsonSerializer.ToJson(dic));
        }

        private class TestClass
        {
            public string field1 = "A";
            public int field2 = 123;
            public string field3 { get { return "B"; } }
            private string field4 = "C";
        }

        [Test]
        public void TestObject()
        {
            Assert.AreEqual("{\"field1\":\"A\",\"field2\":123,\"field3\":\"B\"}", JsonSerializer.ToJson(new TestClass()));
        }

        [Test]
        public void TestDate()
        {
            DateTime date = DateTime.Now;

            DateTime utcBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long utcMs = (long)(date.ToUniversalTime() - utcBase).TotalMilliseconds;

            Assert.AreEqual("new Date(" + utcMs + ")", JsonSerializer.ToJson(date, JsonDateFormat.NewDate));
            Assert.AreEqual("\"Date(" + utcMs + ")\"", JsonSerializer.ToJson(date, JsonDateFormat.Date));
            Assert.AreEqual("\"/Date(" + utcMs + ")/\"", JsonSerializer.ToJson(date, JsonDateFormat.SlashDate));
            Assert.AreEqual("\"\\/Date(" + utcMs + ")\\/\"", JsonSerializer.ToJson(date, JsonDateFormat.EscapedSlashDate));
            Assert.AreEqual("\"" + date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));


            date = new DateTime(1999, 12, 1, 10, 0, 0, DateTimeKind.Utc);

            utcMs = (long)(date.ToUniversalTime() - utcBase).TotalMilliseconds;

            Assert.AreEqual("new Date(" + utcMs + ")", JsonSerializer.ToJson(date,JsonDateFormat.NewDate));
            Assert.AreEqual("\"1999-12-01T10:00:00Z\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));

            date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual("new Date(0)", JsonSerializer.ToJson(date,JsonDateFormat.NewDate));
            Assert.AreEqual("\"1970-01-01T00:00:00Z\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));
        }

        [Test]
        public void TestNull()
        {
            Assert.AreEqual("null", JsonSerializer.ToJson(null));
        }

        [Test]
        public void TestChar()
        {
            Assert.AreEqual("\"x\"", JsonSerializer.ToJson('x'));
        }

        private enum TestEnumeration
        {
            FirstValue,
            SecondValue
        }

        [Test]
        public void TestEnum()
        {
            Assert.AreEqual("\"FirstValue\"", JsonSerializer.ToJson(TestEnumeration.FirstValue));
            Assert.AreEqual("\"SecondValue\"", JsonSerializer.ToJson(TestEnumeration.SecondValue));
        }

        private class TestClass2
        {
            public string Field1 = "abc";
            public int[] Field2 = new[] { 1, 2, 3 };
        }

        [Test]
        public void TestArrayInObject()
        {
            Assert.AreEqual("{\"Field1\":\"abc\",\"Field2\":[1,2,3]}", JsonSerializer.ToJson(new TestClass2()));
        }

        [Test]
        public void TestCircularReferences()
        {
            json_ParentClass obj1 = new json_ParentClass();
            json_ParentClass obj2 = new json_ParentClass();

            obj1.Id = "A";
            obj2.Id = "B";

            obj1.Children = new List<json_ChildClass>()
                                  {
                                      new json_ChildClass
                                          {
                                              Id = "A1",
                                              Parent = obj1,
                                              Others = new List<json_ParentClass>() { obj1,obj2 }
                                          }
                                  };

            string json = JsonSerializer.ToJson(obj1);

            Assert.AreEqual("{\"Id\":\"A\",\"Children\":[{\"Id\":\"A1\",\"Parent\":null,\"Others\":[null,{\"Id\":\"B\",\"Children\":null}]}]}",json);

        }

        private class json_ParentClass
        {
            public string Id;
            public List<json_ChildClass> Children;
        }

        private class json_ChildClass
        {
            public string Id;
            public json_ParentClass Parent;
            public List<json_ParentClass> Others;
        }
    }
}